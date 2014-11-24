using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using System.Diagnostics;


namespace IISSiteGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("started..");
            try
            {
                if (args == null)
                {
                    addEventLog("IIS Site Generator error : invalid parameters", EventLogEntryType.Error);
                    Console.WriteLine("Failed : no parameters supplied.");
                }
                else
                {
                    Console.WriteLine("extracting parameters..");
                    if (args.Length < 3 && args.Length > 4)
                    {
                        addEventLog("IIS Site Generator error : invalid number of parameters", EventLogEntryType.Error);
                        Console.WriteLine("Failed : invalid number of parameters.");
                    }
                    else
                    {
                        Console.WriteLine("initializing server manager..");
                        using (var sm = new ServerManager())
                        {
                            Console.WriteLine("server manager initialized..");
                            try
                            {
                                var command = args[0].ToString();
                                var siteName = args[1].ToString();
                                var newApplicationSiteName = args[2].ToString();
                                string directory = string.Empty;
                                if (args.Length == 4)
                                {
                                    directory = args[3].ToString();
                                }

                                var site = sm.Sites[siteName];
                                switch (command.ToLower())
                                {
                                    case "add":
                                        if (!string.IsNullOrEmpty(directory))
                                        {
                                            addApplication(ref site, newApplicationSiteName, directory);
                                            sm.CommitChanges();
                                        }
                                        break;
                                    case "remove":
                                        removeApplication(ref site, newApplicationSiteName);
                                        sm.CommitChanges();
                                        break;
                                    default:
                                        addEventLog("(IIS Site Generator) Invalid command", EventLogEntryType.Error);
                                        Console.WriteLine("Failed : invalid command.");
                                        break;
                                }
                            }
                            catch(Exception e){
                                addEventLog("IIS Site Generator error generating application : " + e.Message, EventLogEntryType.Error);
                                Console.WriteLine("Failed : " + e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                addEventLog("(IIS Site Generator) An error occured : " + e.Message, EventLogEntryType.Error);
                Console.WriteLine("Failed : " + e.Message);
            }
        }

        private static void addApplication(ref Site site, string newApplicationName, string directory)
        {
            try
            {
                Console.WriteLine("adding application...");
                site.Applications.Add("/" + newApplicationName, directory);
                addEventLog("(IIS Site Generator) Successfully generated site '" + newApplicationName, EventLogEntryType.Information);
                Console.WriteLine("Successfully generated site " + newApplicationName);
            }
            catch (Exception e)
            {
                addEventLog("IIS Site Generator error generating application '" + newApplicationName + "' : " + e.Message, EventLogEntryType.Error);
                Console.WriteLine("Failed : " + e.Message);
            }

        }

        private static void removeApplication(ref Site site, string newApplicationName)
        {
            try
            {
                Console.WriteLine("removing application...");
                var siteToBeRemoved = site.Applications["/" + newApplicationName];
                site.Applications.Remove(siteToBeRemoved);
                Console.WriteLine("Successfully removed site " + newApplicationName);
                
                addEventLog("(IIS Site Generator) Successfully removed site " + newApplicationName, EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                addEventLog("IIS Site Generator error removing application '" + newApplicationName + "' : " + e.Message, EventLogEntryType.Error);
                Console.WriteLine("Failed : " + e.Message);
            }

        }

        private static void addEventLog(string message, EventLogEntryType type)
        {
            var source = ".NET Runtime";
            var application = "Application";
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, application);
            }
            EventLog.WriteEntry(source, message, type);
        }
    }
}
