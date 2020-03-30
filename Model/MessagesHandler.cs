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
        /// <param name="text">The actual message content</param>
        /// <param name="Sender">Info about the sender of the message. If this is null then this is an automatic message</param>
        /// <returns></returns>
        static private Message CreateBaseMessage(string text, Person Sender=null)
        {
            Message newMessage = new Message();

            // Is this an automated message (=has specific sender)?
            if (Sender != null)
            {
                newMessage.senderID = Sender.personID;
            }
            else
            {
                // This is an automated message -> No ID of the sender
                newMessage.senderID = null;
            }

            newMessage.recipientID = null;
            newMessage.recipientClassID = null;

            newMessage.forAllManagement = false;
            newMessage.forAllStudents = false;
            newMessage.forAllTeachers = false;
            newMessage.forEveryone = false;

            newMessage.data = text;
            newMessage.date = DateTime.Now;

            return newMessage;
        }

        /// <summary>
        /// Save a message to the DB
        /// </summary>
        static public void CreateMessage(Message newMessage)
        {
            _mySchoolDB.Messages.Add(newMessage);
            _mySchoolDB.SaveChanges();
        }

        /// <summary>
        /// Creates a new message to a specific person and saves it
        /// </summary>
        static public void CreateMessage(string text, Person recipient, Person sender=null)
        {
            Message newMessage = CreateBaseMessage(text, sender);
            newMessage.recipientID = recipient.personID;
            CreateMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to a specific class and saves it
        /// </summary>
        static public void CreateMessage(string text, Class recipientClass, Person sender = null)
        {
            Message newMessage = CreateBaseMessage(text, sender);
            newMessage.recipientClassID = recipientClass.classID;
            CreateMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to all the teachers and saves it
        /// </summary>
        static public void CreateMessageToTeachers(string text, Person sender = null)
        {
            Message newMessage = CreateBaseMessage(text, sender);
            newMessage.forAllTeachers = true;
            CreateMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to the entire management department and saves it
        /// </summary>
        static public void CreateMessageToManagemet(string text, Person sender = null)
        {
            Message newMessage = CreateBaseMessage(text, sender);
            newMessage.forAllManagement = true;
            CreateMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to all the students and saves it
        /// </summary>
        static public void CreateMessageToStudents(string text, Person sender = null)
        {
            Message newMessage = CreateBaseMessage(text, sender);
            newMessage.forAllStudents = true;
            CreateMessage(newMessage);
        }

        /// <summary>
        /// Creates a new message to everyone and saves it
        /// </summary>
        static public void CreateMessageToEveryone(string text, Person sender = null)
        {
            Message newMessage = CreateBaseMessage(text, sender);
            newMessage.forEveryone = true;
            CreateMessage(newMessage);
        }
        #endregion
    }
}
