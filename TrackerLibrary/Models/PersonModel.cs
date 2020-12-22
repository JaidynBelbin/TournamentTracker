using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {
        /// <summary>
        /// The unique identifer for the person.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The person's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The person's last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// The person's email address to send them tournament info, etc.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// The person's cellphone number, in case they want the information
        /// texted to them.
        /// </summary>
        public string CellphoneNumber { get; set; }

        public string FullName { get => $"{FirstName} {LastName}"; }

        // Default constructor
        public PersonModel() {}

        // Overloaded constructor
        public PersonModel(string firstName, string lastName, string emailAddress, string cellPhoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            CellphoneNumber = cellPhoneNumber;

        }
    }
}
