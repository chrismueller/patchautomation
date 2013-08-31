using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.Security.Principal;

using Altiris.NS;
using Altiris.Resource;
using Altiris.NS.ItemManagement;
using Altiris.NS.ContextManagement;
using Altiris.NS.Security;
using Altiris.Common;
using Altiris.PatchManagementCore.Web;
using Altiris.PatchManagementCore.Policies;
using Symantec.CWoC.APIWrappers;


namespace Symantec.CWoC {
    class ZeroDayPatch {
        private CliConfig config;

        static int Main(string[] args) {
            if (SecurityAPI.is_user_admin()) {
                ZeroDayPatch automate = new ZeroDayPatch();
                automate.config = new CliConfig(config_types.ZeroDayPatch);
                CliInit init;
                if (args.Length == 1 && args[0].StartsWith("/config=")) {
                    init = new CliInit(CliInit.GetConfigFromFile(ref args), ref automate.config);
                } else {
                    init = new CliInit(args, ref automate.config);
                }

                #region If we can initialise run the automate
                if (init.ParseArgs()) {
                    Console.WriteLine("ZeroDayPatch {0} starting.", Constant.VERSION);

                    GuidCollection bulletins = new GuidCollection();
                    bulletins = automate.GetSoftwareBulletins();

                    int i = automate.RunAutomation(bulletins);

                    if (i != -1) {
                        Console.WriteLine("\n{0} software update policy creation tasks were started.", i.ToString());
                        Console.WriteLine("ZeroDayPatch execution completed now. See you soon...");
                        return 0;
                    }
                    return i;
                }
                #endregion
                #region else display the help message
 else {
                    if (automate.config.Print_Version) {
                        Console.WriteLine("{{CWoC}} PatchAutomation version {0}", Constant.VERSION);
                        return 0;
                    }
                    Console.WriteLine(Constant.ZERO_DAY_HELP);
                    if (!automate.config.Help_Needed)
                        return 0;
                    return -1;
                }
 #endregion
            } else {
                Console.WriteLine("Access denied - Only Altiris administrators are allowed to use this tool");
            }
            return -1;
        }

        private int RunAutomation(GuidCollection bulletins) {
            Console.Write("\n\n");

            int i = 0;
            try {
                SecurityContextManager.SetContextData();
                PatchWorkflowSvc wfsvc = new PatchWorkflowSvc();

                string name = "";

                if (config.Dry_Run)
                    Console.WriteLine("\n######## THIS IS A DRY RUN ########");
                foreach (Guid bulletin in bulletins) {
                    name = Item.GetItem(bulletin).Name;
                    Console.WriteLine("");
                    Console.WriteLine("Processing bulletin {0} ({1}) now.", name, bulletin);

                    if (wfsvc.IsStaged(bulletin.ToString())) {
                        Console.WriteLine("\tThis bulletin is already staged.");
                    } else {
                        Console.WriteLine("\t... bulletin will be stagged now.");
                        if (!config.Dry_Run) {
                            wfsvc.EnsureStaged(bulletin.ToString(), true);
                        }
                        Console.WriteLine("\tBulletin is now stagged.");
                    }
                    Console.WriteLine("\tChecking if we need to create a new policy now.");

                    string policyGuid = "";
                    policyGuid = wfsvc.ResolveToPolicies(bulletin.ToString());

                    if (policyGuid == "" || policyGuid.Length == 0 || config.Create_Duplicates) {
                        Console.WriteLine("\t... create a policy for the bulletin now.");
                        if (!config.Dry_Run) {
                            if (config.Target_Guid == "") {
                                PatchAPI wrap = new PatchAPI();
                                wrap.CreateUpdatePolicy(name, bulletin.ToString(), true);
                            } else {
                                PatchAPI wrap = new PatchAPI();
                                wrap.CreateUpdatePolicy(name, bulletin.ToString(), config.Target_Guid, true);
                            }
                            // Added the bulletin to the exclusion list here
                            if (config.Create_Duplicates) {
                                DatabaseAPI.ExecuteNonQuery("insert patchautomation_excluded (bulletin) values ('" + name + "')");
                            }
                            i++;
                        }
                        Console.WriteLine("\tSoftware update policy created!");
                    } else {
                        Console.WriteLine("\tA policy already exists for this bulletin.");
                    }
                    if (i == 10 && config.Test_Run)
                        break;
                }
            } catch (Exception e) {
                Console.WriteLine("Error message={0}z\nInner Exception={1}\nStacktrace={2}", e.Message, e.InnerException, e.StackTrace);
                return -1;
            }
            return i;
        }

        private DataTable GetExcludedBulletins() {
            DatabaseAPI.ExecuteNonQuery(Constant.PATCH_EXCLUSION_CREATION);
            return DatabaseAPI.GetTable(Constant.PATCH_EXCLUSION_QUERY);
        }

        private DataTable GetExistingBulletins() {
            string sp_used;

            if (procedure_installed() && config.Vulnerable) {
                sp_used = @"exec [ZeroDayPatch_GetVulnerableMachines-" + Constant.ZERODAY_SCHEMA_VERSION + "]";
            } else if (config.Patch_All_Vendors) {
                sp_used = @"exec spPMCoreReport_SoftwareBulletinSummary";
            } else if (config.Custom_Procedure != "") {
                sp_used = "if exists (select 1 from sys.objects where type = 'P' and name = '" + config.Custom_Procedure + "')";
                sp_used += "\n\texec [" + config.Custom_Procedure + "]";
            } else {
                sp_used = @"exec spPMCoreReport_AllSoftwareBulletins";
            }
            Console.WriteLine("# Using {0} to get bulletin candidates.", sp_used);

            return DatabaseAPI.GetTable(sp_used);
        }

        private GuidCollection GetSoftwareBulletins() {
            GuidCollection bulletin_collection = new GuidCollection();

            DataTable bulletins = GetExistingBulletins();
            Console.WriteLine("# {0} bulletins returned by the stored procedure execution.", bulletins.Rows.Count);
            DataTable excluded_bulletins = GetExcludedBulletins();
            Console.WriteLine("# {0} bulletin names found in the exclusion table.", excluded_bulletins.Rows.Count);

            if (bulletins.Rows.Count == 0) {
                return bulletin_collection;
            }

            try {
                using (DataTableReader sqlRdr = bulletins.CreateDataReader()) {
                    #region Get position of the used field
                    int pos_released = -1;
                    int pos_res_guid = -1;
                    int pos_bulletin = -1;
                    int pos_severity = -1;
                    int pos_vendor = -1;

                    for (int i = 0; i < sqlRdr.FieldCount; i++) {
                        string field_name = sqlRdr.GetName(i).ToLower();
                        if (field_name == "released")
                            pos_released = i;
                        if (field_name == "_resourceguid")
                            pos_res_guid = i;
                        if (field_name == "bulletin")
                            pos_bulletin = i;
                        if (field_name == "severity")
                            pos_severity = i;
                        if (field_name == "vendor")
                            pos_vendor = i;
                    }

                    bool field_init = false;
                    if (pos_severity != -1 && pos_res_guid != -1 && pos_released != -1 && pos_bulletin != -1)
                        field_init = true;
                    #endregion

                    if (config.Debug)
                        Console.WriteLine("# Field positions are:\n\tBulletin={0}\n\tReleased={1}\n\tResourceGuid={2}\n\tSeverity={3}", pos_bulletin, pos_released, pos_res_guid, pos_severity);

                    if (field_init) {
                        while (sqlRdr.Read()) {
                            Guid bguid = sqlRdr.GetGuid(pos_res_guid);
                            String bull_name = sqlRdr.GetString(pos_bulletin);
                            String sev = sqlRdr.GetString(pos_severity);
                            DateTime dt = sqlRdr.GetDateTime(pos_released);
                            String bull_vendor = string.Empty;
                            if (pos_vendor != -1)
                                bull_vendor = sqlRdr.GetString(pos_vendor).ToLower();

                            bool row_excluded = false;

                            #region // Break if the current bulletin is excluded
                            foreach (DataRow r in excluded_bulletins.Rows) {
                                if (r[0].ToString() == bull_name) {
                                    row_excluded = true;
                                    break;
                                }
                            }

                            if (row_excluded) {
                                continue;
                            }
                            #endregion

                            if (config.Debug)
                                Console.WriteLine("Bulletin guid={0}, severity={1}, released={2}", bguid, sev, dt.ToString("yyyy-MM-dd"));
                            if ((sev.ToLower() == config.Severity.ToLower() || config.Severity == "*") && dt >= config.Released_After && dt <= config.Released_Before ) {
                                if (config.Debug)
                                    Console.WriteLine("### WE HAVE A MATCH ###");
                                if (pos_vendor == -1 || config.Vendor_Name == bull_vendor || config.Vendor_Name == "*")
                                bulletin_collection.Add(bguid);
                            }
                        }
                    } else {
                        Console.WriteLine("Failed to find the required fields in the provided data table. Not doing anything.");
                    }
                }
            } catch (Exception e) {
                Console.WriteLine("Error: {0}\nException message = {1}\nStack trace = {2}.", e.Message, e.InnerException, e.StackTrace);
            }
            Console.WriteLine("{0} bulletins match the {1} severity and will be checked for policies.", bulletin_collection.Count, config.Severity);
            return bulletin_collection;
        }

        public bool procedure_installed() {

            string test_sql = @"select 1 from sysobjects where type = 'P' and name = 'ZeroDayPatch_GetVulnerableMachines-" + Constant.ZERODAY_SCHEMA_VERSION + "'";

            if (DatabaseAPI.ExecuteScalar(test_sql) == 1) {
                return true;
            }

            DatabaseAPI.ExecuteNonQuery(Constant.ZERODAY_GET_VULNERABLE);

            foreach (string legacy_sp in legacy_spnames) {
                string clean_legacy = "if exists (select 1 from sysobjects where type = 'P' and name = '" + legacy_sp + "') "
                    + "begin "
                    + "drop proc [" + legacy_sp + "] "
                    + "end";
                Console.WriteLine("Making sure legacy stored procedure {0} is not present.", legacy_sp);
                DatabaseAPI.ExecuteNonQuery(clean_legacy);
            }
            return true;
        }

        public string[] legacy_spnames = new string[]{
            "spBulletinsWithVulnerableClients",
            "ZeroDayPatch_GetVulnerableMachines-0001",
            "ZeroDayPatch_GetVulnerableMachines-0002"
            };
    }
}
