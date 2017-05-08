#region Imports

using System;
using System.Collections.Generic;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.Configuration;
using System.IO;
using System.Linq;
using ServiceStack.Text;

#endregion

namespace EmailParser
{
    public class POPEmailClientAdapter : IPOPEmailClientAdapter
    {
        /// <summary>
        /// Action to delete email messages based on message number - unique id for each email
        /// </summary>
        /// <param name="messageNumber"></param>
        /// <returns></returns>
        public bool DeleteMessageOnServer(int messageNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action to fetch all email messages
        /// </summary>
        /// <returns></returns>
        public List<Message> FetchAllMessages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action to fetch unseen messages
        /// </summary>
        /// <param name="seenUids"></param>
        /// <returns></returns>
        public List<Message> FetchUnseenMessages(List<string> seenUids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action to process unseen messages
        /// </summary>
        /// <returns></returns>
        public bool ProcessUnseenMessages()
        {
            try
            {
                // The client disconnects from the server when being disposed
                using (Pop3Client client = new Pop3Client())
                {
                    string hostname = ConfigurationManager.AppSettings["hostname"];
                    int port = int.Parse(ConfigurationManager.AppSettings["port"]);
                    string username = ConfigurationManager.AppSettings["useremail"];
                    string password = ConfigurationManager.AppSettings["password"];
                    string processeduidpath  = string.Empty;
                    string processedemaildatapath = string.Empty;

                    //Connecting to Pop3
                    client.Connect(hostname, port, true);

                    //Authenticate Pop3
                    client.Authenticate(username, password);

                    //Create directory if already exists return the path for the processed files
                    string processedEmailMessageIdFiles = CreateDirectory("ProcessedEmailIdFile");

                    //Get list of files inside the directory to get all the processed emailsuniqueid
                    string[] files = Directory.GetFiles(processedEmailMessageIdFiles);

                    if(files.Length == 0)
                    {
                        //Create file if no file exists
                        processeduidpath =  Path.Combine(processedEmailMessageIdFiles, "processeduid.txt");
                        processedemaildatapath =  Path.Combine(processedEmailMessageIdFiles, "processed.txt");

                        File.Create(processeduidpath).Dispose();
                        File.Create(processedemaildatapath).Dispose();
                    }

                    // For now assuming only one file would their with the list of processed emailsuniqueid
                    List<string> processeduids = File.ReadAllLines(Path.Combine(processedEmailMessageIdFiles, "processeduid.txt")).ToList();

                    // New List to write to file after processing the emails
                    List<string> newprocesseduids = new List<string>();

                    // Create a list we can return with all new messages
                    List<Message> newMessages = new List<Message>();

                    // Create a list of new messages read to store it in a file
                    List<EmailModel> newEmailMessages = new List<EmailModel>(); 

                    // Fetch all the current uids seen
                    List<string> uids = client.GetMessageUids();

                    //Looping through the list of emails
                    for (int i = 0; i < uids.Count; i++)
                    {
                        string currentUidOnServer = uids[i];
                        if (!processeduids.Contains(currentUidOnServer))
                        {
                            // We have not seen this message before.
                            // Download it and add this new uid to seen uids

                            // the uids list is in messageNumber order - meaning that the first
                            // uid in the list has messageNumber of 1, and the second has 
                            // messageNumber 2. Therefore we can fetch the message using
                            // i + 1 since messageNumber should be in range [1, messageCount]
                            Message unseenMessage = client.GetMessage(i + 1);

                            var emailMessage = new EmailModel
                            {
                                FromEmail = unseenMessage.Headers.From.MailAddress.ToString(),
                                ToEmail = unseenMessage.Headers.To,
                                Subject = unseenMessage.Headers.Subject,
                                DateTimeSent = unseenMessage.Headers.DateSent,
                                Date = unseenMessage.Headers.Date,
                                Body = unseenMessage.MessagePart.MessageParts[0].GetBodyAsText(),
                                BodyHTMLText = unseenMessage.MessagePart.MessageParts[1].GetBodyAsText(),
                            };

                            //// Add the message to the new messages
                            //newMessages.Add(unseenMessage);
                            newEmailMessages.Add(emailMessage);

                            // Add the uid to the seen uids, as it has now been seen
                            newprocesseduids.Add(currentUidOnServer);
                        }
                    }

                    // Write all the processed uniqueemailid to the file to remove processing of the same emailid.
                    File.WriteAllLines(Path.Combine(processedEmailMessageIdFiles, "processeduid.txt"), newprocesseduids);
                    //foreach(var message in newEmailMessages)
                    //{
                        File.WriteAllText(Path.Combine(processedEmailMessageIdFiles, "processed.txt"), CsvSerializer.SerializeToCsv<EmailModel>(newEmailMessages));
                    //}
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public string CreateDirectory(string foldername)
        {
            try
            {
                string curpath = Directory.GetCurrentDirectory();
                string folder = curpath + "\\" + foldername;

                // If the folder is not existed, create it.
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                return folder;
            }

            catch (Exception ex)
            {

                return string.Empty;
            }
        }

    }
}
