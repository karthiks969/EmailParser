#region Imports

using OpenPop.Mime;
using System.Collections.Generic;

#endregion

namespace EmailParser
{
    interface IPOPEmailClientAdapter
    {
        /// <summary>
        /// Action to get all the email messages.
        /// </summary>
        /// <returns></returns>
        List<Message> FetchAllMessages();

        /// <summary>
        /// Action to delete the email message on server.
        /// </summary>
        /// <param name="messageNumber"></param>
        /// <returns></returns>
        bool DeleteMessageOnServer(int messageNumber);

        /// <summary>
        /// Action to get only the messages that were not parsed before using the Id.
        /// </summary>
        /// <param name="seenUids"></param>
        /// <returns></returns>
        List<Message> FetchUnseenMessages(List<string> seenUids = null);

        /// <summary>
        /// Action to process unseen email messages
        /// </summary>
        /// <returns></returns>
        bool ProcessUnseenMessages();

        /// <summary>
        /// Action to create directory
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
        string CreateDirectory(string foldername);
    }
}
