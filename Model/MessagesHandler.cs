using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MySchoolYear.Model
{
    /// <summary>
    /// Handles creation of new messages
    /// </summary>
    public static class MessagesHandler
    {
        #region Fields
        private static SchoolEntities _mySchoolDB;
        #endregion

        #region Constructors
        static MessagesHandler()
        {
            _mySchoolDB = new SchoolEntities();
        }
        #endregion

        #region Methods
        /// <summary>
        /// An helper method that creates a simple template of a message 
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual message content</param>
        /// <param name="senderID">The ID of the sender. If this is null then this is an automatic message</param>
        /// <returns></returns>
        static private Message CreateBaseMessage(string title, string text, int? senderID=null)
        {
            Message newMessage = new Message();

            newMessage.senderID = senderID;
            newMessage.recipientID = null;
            newMessage.recipientClassID = null;

            newMessage.forAllManagement = false;
            newMessage.forAllStudents = false;
            newMessage.forAllTeachers = false;
            newMessage.forEveryone = false;

            newMessage.title = title;
            newMessage.data = text;
            newMessage.date = DateTime.Now;

            return newMessage;
        }

        /// <summary>
        /// Save a message to the DB
        /// </summary>
        static public void SaveMessage(Message newMessage)
        {
            _mySchoolDB.Messages.Add(newMessage);
            _mySchoolDB.SaveChanges();
        }

        /// <summary>
        /// Creates a new message to a specific person and saves it
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual content of the message</param>
        /// <param name="recipientID">Person ID of the recipient</param>
        /// <param name="sender">Person ID of the sender. If null, it is an automatic message</param>
        static public void CreateMessageToPerson(string title, string text, int recipientID, int? sender=null)
        {
            Message newMessage = CreateBaseMessage(title, text, sender);
            newMessage.recipientID = recipientID;
            SaveMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to a specific class and saves it
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual content of the message</param>
        /// <param name="recipientClassID">Person ID of the recipient</param>
        /// <param name="senderID">Person ID of the sender. If null, it is an automatic message</param>
        static public void CreateMessageToClass(string title, string text, int recipientClassID, int? senderID = null)
        {
            Message newMessage = CreateBaseMessage(title, text, senderID);
            newMessage.recipientClassID = recipientClassID;
            SaveMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to all the teachers and saves it
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual content of the message</param>
        /// <param name="senderID">Person ID of the sender. If null, it is an automatic message</param>
        static public void CreateMessageToTeachers(string title, string text, int? senderID = null)
        {
            Message newMessage = CreateBaseMessage(title, text, senderID);
            newMessage.forAllTeachers = true;
            SaveMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to the entire management department and saves it
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual content of the message</param>
        /// <param name="senderID">Person ID of the sender. If null, it is an automatic message</param>
        static public void CreateMessageToAllManagement(string title, string text, int? senderID = null)
        {
            Message newMessage = CreateBaseMessage(title, text, senderID);
            newMessage.forAllManagement = true;
            SaveMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to all the students and saves it
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual content of the message</param>
        /// <param name="senderID">Person ID of the sender. If null, it is an automatic message</param>
        static public void CreateMessageToAllStudents(string title, string text, int? senderID = null)
        {
            Message newMessage = CreateBaseMessage(title, text, senderID);
            newMessage.forAllStudents = true;
            SaveMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to everyone and saves it
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="text">The actual content of the message</param>
        /// <param name="senderID">Person ID of the sender. If null, it is an automatic message</param>
        static public void CreateMessageToEveryone(string title, string text, int? senderID = null)
        {
            Message newMessage = CreateBaseMessage(title, text, senderID);
            newMessage.forEveryone = true;
            SaveMessage(newMessage);
        }
        #endregion
    }
}
