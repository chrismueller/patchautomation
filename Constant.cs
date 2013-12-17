using System;
using System.Collections.Generic;
using System.Text;

namespace Symantec.CWoC
{
    class Constant
    {
        public const string VERSION = "9b";
        public const string ZERODAY_SCHEMA_VERSION = "0003";

        #region SQL STRINGS
        public const string PATCH_EXCLUSION_QUERY = @"if exists (select 1 from sys.objects where name = 'patchautomation_excluded') select bulletin from patchautomation_excluded";

        public const string PATCH_EXCLUSION_CREATION = @"
            IF NOT EXISTS(select 1 from sys.objects where type ='U' and name = 'PatchAutomation_Excluded')
            BEGIN
            CREATE TABLE [PatchAutomation_Excluded](
	            [_id] [int] IDENTITY(1,1) NOT NULL,
	            [Bulletin] [nvarchar](255) NOT NULL,
	            [CreateDate] [datetime] NULL,
             CONSTRAINT [pk_PatchAutomation_Excluded] PRIMARY KEY CLUSTERED
            (
	            [Bulletin] ASC
            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            ) ON [PRIMARY]

            ALTER TABLE [PatchAutomation_Excluded] ADD  DEFAULT (getdate()) FOR [CreateDate]
            END
            ";
        #endregion

        #region private const string COMMON_FEATURES
        private const string COMMON_FEATURES = @"    /config=<file path>
        Reads the file at the provided path and parses each line for com-
        -mand line options. Here is a sample config file content:
            /severity=critical
            /custom-sp=CWoC_GetAllBulletins
            /vendor=google
            /dryrun
            /debug

    /test   
        Run the automate in test mode only. A maximum of 10 policies will
        be created in this mode.

    /dryrun 
        Run the automate in dry run mode. No changes will be made to the 
        system, but expected operation will be printed to the console.

    /severity=<severity>|*
        Set the severity used to select bulletins that will be handle by 
        the automate. The * wildcard can be use to match all severities.

    /patchall
        Use this command line if you want to manage bulletins from all
        vendors in the database. By default we only handle Microsoft bul-
        -letins.

    /released-before=<yyyy-MM-dd formatted date>
        Configure a date filter that will include bulletin released before
        the specified date. It is set by default to the current date.

    /released-after=<yyyy-MM-dd formatted date>
        Configure a date filter that will include bulletin released after
        the specified date. It is set by default to (current date -1 year).

    /custom-sp=<sp_name>
        This option allows the user to specify a custom stored procedure to
        be called during the execution. The stored procedure may be present
        on the database (if not the automate will return with no errors) and
        must contains the following columns that are used and needed:
            * _resourceguid [Software bulletin guid]
            * released [Software bulletin release date]
            * bulletin [Bulletin name]
            * severity [Bulletin Severity]
        You can also add a vendor column if you want to filter bulletins by
        vendor (see option /vendor)

    /vendor=<vendor string>|*
        Configure a vendor filter to only return bulletins that match the
        vendor string from a custom procedure. This is because the vendor
        field doesn't exist in default Patch Procedures used by this tool.

        If /vendor is specified with a custom-sp that doesn't contain the
        vendor field the setting will be ignored (all bulletins will be
        returned).

    /debug
        Output extra information on the command line to allow debugging or
        reporting problems to Symantec Connect.

    /duplicates
        Use this command if you want the tool to generate duplicate
        policies. This is useful if you want, for example, to migrate
        policies from a parent to a child SMP without disruption.

        Note! Duplicated and new entries will be added to the exclusion 
        table in the database for safety reasons.

    /exclude-on-fail
        Use this command to add bulletins to the excluded table if it fails
        3 times during the stagging or policy creation phases. If not uses
        the failing bulletin will only be skipped.

    /retarget
        Use this command if you want to switch existing policies to use a
        new target. The target guid should be provided with /targetguid=...

    /version
        Print out the current version of the tool.

    /?
        Print this help message to the console (stdout).
";
        #endregion

        #region public const string ZERO_DAY_HELP
        public const string ZERO_DAY_HELP = @"
ZeroDayPatch (version " + VERSION + @") command line usage:

    /vulnerable
            Use this command line switch to install and run a custom stored
            procedure to retrieve candidate bulletins. The procedure will be
            installed is and named ZeroDayPatch_GetVulnerableMachines-" + ZERODAY_SCHEMA_VERSION + @".

    /targetguid=<target_guid>
            Use this option to set the target guid to be used with newly 
            created policies. This will over-write the default target defined
            globally.

" + COMMON_FEATURES;
        #endregion

        #region public const string PATCH_AUTOMATE_HELP
        public const string PATCH_AUTOMATE_HELP = @"
PatchAutomate (version " + VERSION + @") command line usage:

    /targetguid-test=<target_guid>
    /targetguid-validation=<target_guid>
    /targetguid-production=<target_guid>

        The 3 command line options above are mandatory and used to define
        the target used with each phases (Test, Validation, Production).

    /day2validation=<n>
    /day2production=<n>
        The 2 command line above define the interval in days necessary to
        move policies from one phase to the next. The days are match using
        >= n, so if you set these to 1 (day) the policies created the day
        before will be re-targeted (so the interval could be as low as a
        few minutes if the tool runs at 2300 and again at 0005 the next day.

    /fr
        Switch the automatic policy names postfix from English to French like:
            * 'Test Target' <--> 'Cible de Test'
            * 'Validation Rarget' <--> 'Cible de Validation'
            * 'Production Target' <--> 'Cible de Production'

" + COMMON_FEATURES;
        #endregion

        #region public const string ZERODAY_GET_VULNERABLE
        public const string ZERODAY_GET_VULNERABLE = @"
-- ============================================================================
-- Author:		Ludovic FERRE, Symantec SAS
-- Create date: 2012-11-12
-- Description:	Custom sp to return distinct updates that have vulnerable PC
-- base on spPMWindows_ComplianceByBulletin
-- ============================================================================
CREATE PROCEDURE [ZeroDayPatch_GetVulnerableMachines-" + ZERODAY_SCHEMA_VERSION + @"]
AS
BEGIN

	SELECT LOWER(SUBSTRING( sts.StringRef, 10, 100 )) AS [RefName], sts.[String]
	  INTO #tmpSeverityString
	  FROM (
				SELECT   BaseGuid, StringRef, MAX(Priority) AS [Priority]
				  FROM [dbo].[String]                       s
				  JOIN [dbo].[fnGetBaseCultures]('en-US') f
					ON f.Culture = s.Culture
				 WHERE [BaseGuid] = '746A8B51-A570-43AB-8D36-AEF36D0C8041'
				   AND [StringRef] LIKE 'severity.%'
				 GROUP BY [BaseGuid], [StringRef]
			 ) ref
	  JOIN [dbo].[fnGetBaseCultures]('en-US') fnc
	    ON fnc.[Priority] = ref.[Priority]
	  JOIN [dbo].[String] sts
	    ON sts.[BaseGuid] = ref.[BaseGuid]
	   AND sts.[StringRef] = ref.[StringRef]
	   AND sts.[Culture]   = fnc.[Culture]

	SELECT swb._ResourceGuid, swb.FirstReleaseDate AS [Released], ISNULL( tss.[String], srl.SeverityName ) AS [Severity]
	  INTO #tmpBulletinNames
	  FROM Inv_Software_Bulletin swb
	  JOIN ItemActive asb
		ON asb.Guid = swb._ResourceGuid
	  JOIN Inv_PM_Severity_Rating sr
		ON sr._ResourceGuid  = swb._ResourceGuid
	   AND sr.SeverityRatingSystemGuid <> '6CCEF81F-F791-4DC4-8FC6-90D149FC0187'
	  JOIN Inv_Severity_Rating_Level       srl
		ON srl._ResourceGuid = sr.SeverityRatingSystemGuid
	   AND srl.SeverityLevel = sr.SeverityLevel
	  LEFT JOIN #tmpSeverityString tss
		ON tss.RefName = LOWER(srl.SeverityName)

	  DROP TABLE #tmpSeverityString

	SELECT cid._ResourceGuid
	  INTO #tempScopedResources
	  FROM Inv_AeX_AC_Identification cid 
	  LEFT JOIN vPMCore_GetAllRetiredMachines ret
	    ON ret.Guid = cid._ResourceGuid
	 WHERE ret.Guid IS NULL -- exclude the retired machine

	SELECT cb1.BulletinGuid, COUNT(cb1._ResourceGuid)AS Applicable, SUM(cb1.Installed) AS Installed
	  INTO #tmpBulletinCnt
	  FROM (
			SELECT bul._ResourceGuid AS [BulletinGuid], sua._ResourceGuid, CASE WHEN COUNT(sua.UpdateGuid) = SUM( CASE WHEN sui.UpdateGuid IS NULL THEN 0 WHEN ses.PendingSince IS NULL THEN 1 ELSE 0 END ) THEN 1 ELSE 0 END   AS [Installed]
			  FROM vPMWindows_UpdateApplicable sua
			  JOIN ResourceAssociation b2u 
				ON b2u.ChildResourceGuid = sua.UpdateGuid
			   AND b2u.ResourceAssociationTypeGuid = '7EEAB03A-839C-458D-9AF2-55DB6B173293'	-- SWB to SWU
			  JOIN #tmpBulletinNames bul ON bul._ResourceGuid = b2u.ParentResourceGuid
			  JOIN #tempScopedResources res ON res._ResourceGuid = sua._ResourceGuid
			  LEFT JOIN vPMWindows_UpdateInstalled  sui ON sui.UpdateGuid    = sua.UpdateGuid
			   AND sui._ResourceGuid = sua._ResourceGuid
			  LEFT JOIN vPMCore_ComputersPendingRebootByPackage ses 
				ON ses.SoftwareUpdateGuid = sua.UpdateGuid
			   AND ses._ResourceGuid = sua._ResourceGuid
			 WHERE sua.UpdateGuid NOT IN -- filter out supersede applicable updates
					(
						SELECT DISTINCT ChildResourceGuid
						  FROM ResourceAssociation
						 WHERE ResourceAssociationTypeGuid = '644A995E-211A-4D94-AA8A-788413B7BE5D'
					)
			 GROUP BY bul._ResourceGuid, sua._ResourceGuid
			) AS cb1
	 GROUP BY cb1.BulletinGuid

	  DROP TABLE #tempScopedResources

	SELECT  distinct(swb._ResourceGuid)       AS [_ResourceGuid],
				it.Name                 AS [Bulletin],
				swb.Severity            AS [Severity],
				swb.Released            AS [Released],
				cbb.Applicable          AS [Applicable (Count)],
                cbb.Installed	        AS [Installed (Count)]
	  FROM #tmpBulletinNames swb
	  JOIN #tmpBulletinCnt cbb
	    ON cbb.BulletinGuid = swb._ResourceGuid
	  JOIN RM_ResourceSoftware_Bulletin it
	    ON it.Guid = swb._ResourceGuid
	  LEFT JOIN vPMCore_SeverityRating csr
	    ON csr._ResourceGuid = swb._ResourceGuid
	   AND csr.SeverityRatingSystemGuid = '6CCEF81F-F791-4DC4-8FC6-90D149FC0187' -- Custom Sev Rating
	 WHERE cbb.Applicable > cbb.Installed
	 ORDER BY [Released] DESC, Bulletin DESC

	  DROP TABLE #tmpBulletinNames
	  DROP TABLE #tmpBulletinCnt
END
";
        #endregion

    }
}
