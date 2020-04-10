﻿using System;
using System.Collections.Generic;
using System.Linq;
using MySchoolYear.Model;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// Viewing incoming messages
    /// </summary>
    public class MessagesDisplayViewModel : BaseViewModel, IScreenViewModel
    {
        #region SubStructs
        public class DisplayedMessage
        {
            public string SenderName { get; set; }
            public string RecipientName { get; set; }
            public DateTime MessageDateTime { get; set; }
            public string MessageDate 
            { 
                get
                {
                    return MessageDateTime.ToString("dd/MM/yyyy");
                }
            }
            public string Title { get; set; }
            public string Details { get; set; }
        }

        private enum MessageRecipientType
        {
            Direct,
            Class,
            Students,
            Teachers,
            Management,
            Everyone,
        }
        #endregion

        #region Fields
        #endregion

        #region Properties / Commands
        // Base Properties
        public Person ConnectedUser { get; }
        public bool HasRequiredPermissions { get; }
        public string ScreenName { get { return "הצגת הודעות"; } }

        // Business Logic Properties
        public List<DisplayedMessage> Messages { get; set; }
        #endregion

        #region Constructors
        public MessagesDisplayViewModel(Person currentUser)
        {
            ConnectedUser = currentUser;
            HasRequiredPermissions = true;
        }
        #endregion

        #region Methods
        // Gather the messages that are relevent to the connected user.
        public void Initialize()
        {
            // Reset data first
            Messages = new List<DisplayedMessage>();
            var schoolMessages = new SchoolEntities().Messages;

            // Start by gathering the messages that are for everyone in the school or directly to the connected user.
            schoolMessages.Where(message => message.forEveryone).ToList()
                .ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Everyone)));
            schoolMessages.Where(message => message.recipientID == ConnectedUser.personID).ToList()
                .ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Direct)));

            // Gather messages depending on the user permissions
            if (ConnectedUser.isStudent)
            {
                // User is a student => Gather messages for their class, and messages for all students
                schoolMessages.Where(message => message.recipientClassID == ConnectedUser.Student.classID)
                    .ToList().ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Class)));
                schoolMessages.Where(message => message.forAllStudents).ToList()
                    .ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Students)));
            }
            if (ConnectedUser.isParent)
            {
                // User is a parent => gather messages for the class of his children (but not direct messages to them - keeping those private)
                schoolMessages.Where(message => ConnectedUser.ChildrenStudents.Any(childStudent => childStudent.classID == message.recipientClassID))
                    .ToList().ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Class)));
            }
            if (ConnectedUser.isTeacher)
            {
                // Gather messages for all teachers
                schoolMessages.Where(message => message.forAllTeachers).ToList()
                    .ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Teachers)));

                if (ConnectedUser.Teacher.classID != null)
                {
                    // User is an homeroom teacher - gather messages for his class
                    schoolMessages.Where(message => ConnectedUser.Teacher.classID == message.recipientClassID).ToList()
                        .ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Class)));
                }
            }
            if (ConnectedUser.isSecretary || ConnectedUser.isPrincipal)
            {
                // Gather messages for management
                schoolMessages.Where(message => message.forAllManagement).ToList()
                    .ForEach(message => Messages.Add(ModelMessageToDisplayedMessage(message, MessageRecipientType.Management)));
            }

            // Order messages by date (latest first)
            Messages = Messages.OrderByDescending(message => message.MessageDateTime).ToList();
        }

        /// <summary>
        /// Converts the Model's Message object in a local DisplayedMessage object
        /// </summary>
        /// <param name="message">The message to convert</param>
        /// <param name="messageType">The message to convert</param>
        /// <returns>DisplayedMessage version of the message</returns>
        private DisplayedMessage ModelMessageToDisplayedMessage(Message message, MessageRecipientType messageType)
        {
            // Make sure input is correct
            if (message == null)
            {
                throw new ArgumentNullException("Arguement 'message' cannot be null!");
            }

            DisplayedMessage displayedMessage = new DisplayedMessage();

            // Get the name of the sender (unless it is an automatic message)
            if (message.senderID != null)
            {
                displayedMessage.SenderName = message.SenderPerson.firstName + " " + message.SenderPerson.lastName;
            }
            else
            {
                displayedMessage.SenderName = "הודעה אוטומטית";
            }

            displayedMessage.RecipientName = GetRecipientName(message, messageType);
            displayedMessage.Title = message.title;
            displayedMessage.MessageDateTime = message.date;
            displayedMessage.Details = message.data;

            return displayedMessage;
        }
        
        /// <summary>
        /// Assistant method that creates the recipient name for a message depending on the message type
        /// </summary>
        /// <param name="message">The source message</param>
        /// <param name="messageType">The type of recipient (class, everyone, specific person...)</param>
        /// <returns>The fitting recipient name</returns>
        private static string GetRecipientName(Message message, MessageRecipientType messageType)
        {
            string recipientName;
            switch (messageType)
            {
                case MessageRecipientType.Class:
                    recipientName = "כיתה " + message.Class.className;
                    break;
                case MessageRecipientType.Everyone:
                    recipientName = "הודעה כללית";
                    break;
                case MessageRecipientType.Management:
                    recipientName = "הנהלה";
                    break;
                case MessageRecipientType.Students:
                    recipientName = "תלמידים";
                    break;
                case MessageRecipientType.Teachers:
                    recipientName = "מורים";
                    break;
                case MessageRecipientType.Direct:
                default:
                    recipientName = message.ReceiverPerson.firstName + " " + message.ReceiverPerson.lastName;
                    break;
            }

            return recipientName;
        }
        #endregion
    }
}
