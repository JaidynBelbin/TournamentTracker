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
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {

        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();
        public CreateTournamentForm()
        {
            InitializeComponent();

            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentPlayersListBox.DataSource = null;
            tournamentPlayersListBox.DataSource = selectedTeams;
            tournamentPlayersListBox.DisplayMember = "TeamName";

            prizeslistBox.DataSource = null;
            prizeslistBox.DataSource = selectedPrizes;
            prizeslistBox.DisplayMember = "PlaceName";
        }

        // Moving the selected team from the dropdown into the ListBox.
        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }

        private void deleteSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentPlayersListBox.SelectedItem;

            if (t != null)
            {
                availableTeams.Add(t);
                selectedTeams.Remove(t);

                WireUpLists();
            }
        }

        private void deleteSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizeslistBox.SelectedItem;

            if (p != null)
            {
                selectedPrizes.Remove(p);

                // TODO - Create some way of either saving the already created Prizes and allowing the user
                // to select one, or delete the Prize upon removing it from this listBox.
                WireUpLists();
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Opening the CreatePrizeForm and passing in this instance of the CreateTournamentForm.
            // An example of 'loosely binding' two forms together.

            CreatePrizeForm form = new CreatePrizeForm(this);
            form.Show();
        }

        // The CreatePrizeForm and CreateTeamForm classes, and the completed models are the ones that we
        // fill out in the form.
        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
            WireUpLists();
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm form = new CreateTeamForm(this);
            form.Show();
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            // Need to check if the entered entry fee is able to be converted to a decimal: if yes, output it to the variable 'fee'.
            // If not, display an error message. We don't want the program to crash at this point.

            bool feeValid = decimal.TryParse(entryFeeValue.Text, out decimal fee);

            if (!feeValid)
            {
                MessageBox.Show("You need to enter a valid entry fee.", 
                    "Invalid Fee", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                return;
            }

            // Creating our tournament model.

            TournamentModel tournament = new TournamentModel
            {
                TournamentName = tournamentNameValue.Text,
                EntryFee = fee,
                Prizes = selectedPrizes,
                EnteredTeams = selectedTeams
            };

            TournamentLogic.CreateRounds(tournament);
                               

            // Create Tournament entry
            // Create all of the prize entries
            // Create all of the team entries

            // Create our matchups of teams

            GlobalConfig.Connection.CreateTournament(tournament);
        }
    }
}
