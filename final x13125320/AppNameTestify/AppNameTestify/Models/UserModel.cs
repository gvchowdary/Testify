using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppNameTestify.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the sur.
        /// </summary>
        /// <value>
        /// The name of the sur.
        /// </value>
        public string SurName { get; set; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        public static List<UserModel> getUsers()
        {
            List<UserModel> users = new List<UserModel>()
            {
                new UserModel (){ ID=1, Name="Anubhav", SurName="Chaudhary" }, 
                new UserModel (){ ID=2, Name="Mohit", SurName="Singh" },
                new UserModel (){ ID=3, Name="Sonu", SurName="Garg" },
                new UserModel (){ ID=4, Name="Shalini", SurName="Goel" },
                new UserModel (){ ID=5, Name="James", SurName="Bond" },
            };
            return users;
        }
    }
}