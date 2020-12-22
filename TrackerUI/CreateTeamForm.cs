using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {

        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();

        public CreateTeamForm()
        {
            InitializeComponent();

            //CreateSampleData();

            WireUpLists();
        }

        private void WireUpLists()
        {
            // TODO - look at better way of refreshing data sources. Maybe use onPropertyChanged?
            selectTeamMemberDropDown.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }

        // Just to populate the Views for testing purposes
        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "John", LastName = "Corey" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Richard", LastName = "Foreman" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Mark", LastName = "Wilder" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "Joe", LastName = "Smith" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Matt", LastName = "Baldwin" });
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel person = new PersonModel(firstNameValue.Text,
                    lastNameValue.Text,
                    emailValue.Text,
                    cellphoneValue.Text);

                person = GlobalConfig.Connection.CreatePerson(person);

                // Adding the newly created member to the available people drop down.
                availableTeamMembers.Add(person);

                // Refreshing the dropdown and the list box.
                WireUpLists();

                // Resetting the values to default.
                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";

                MessageBox.Show("Team member successfully created!");
            }
            else
            {
                MessageBox.Show("You need to fill in all the fields.");
            }
        }


        private bool ValidateForm()
        {
           
            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }

            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }

            if (emailValue.Text.Length == 0)
            {
                return false;
            }

            if (cellphoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        // When a person is selected from the dropdown, add that person to
        // the teamMembersListBox and a List<PersonModel> to set to the TeamMembers list.
        // User can add as many existing team members as they want to the list.

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            // Checking to make sure someone is selected to prevent crashes.
            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);
            }

            WireUpLists();
        }
       

        private void deleteSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            // Checking to make sure someone is selected to prevent crashes.
            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);
            }

            WireUpLists();
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            // The team consists of all the people selected in the Listbox.

            TeamModel team = new TeamModel();

            // Getting the name from the textbox, and the team members from the Listbox.

            if (teamNameValue.Text.Length > 0 && selectedTeamMembers.Count > 0)
            {
                team.TeamName = teamNameValue.Text;
                team.TeamMembers = selectedTeamMembers;

                // Creating the team.
                GlobalConfig.Connection.CreateTeam(team);

                MessageBox.Show($"{teamNameValue.Text} has been created!");

            } else
            {
                MessageBox.Show("Please provide a team name and \nselect at least one team member!");
            }
            
            // TODO - If not closing the form after creation, reset the form.
        }
    }
}
