using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        private List<int> rounds = new List<int>();
        private List<MatchupModel> selectedMatchups = new List<MatchupModel>();

        BindingSource roundsBinding = new BindingSource();
        BindingSource matchupsBinding = new BindingSource();

        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();

            // Passing the selected tournament from the Dashboard form when creating a new instance of this Form.
            tournament = tournamentModel;

            // Loading the tournament name.
            LoadFormData();

            // Loading the rounds into the dropdown, which triggers the matches to load into the ListBox and the data to display
            // for each match.
            LoadRounds();
        }

        private void LoadFormData()
        {
            // Setting the tournament name
            tournamentName.Text = tournament.TournamentName;
        }

        private void LoadRounds()
        {
            // List to hold the round numbers in the tournament
            rounds = new List<int>();

            // Will always have at least one round
            rounds.Add(1);
            int currentRound = 1;

            foreach (List<MatchupModel> match in tournament.Rounds)
            {
                // Looping through all the other existing round numbers in the tournament and adding them.
                if (match.First().MatchupRound > currentRound)
                {
                    currentRound = match.First().MatchupRound;
                    rounds.Add(currentRound);
                }
            }

            // Setting the round dropdown data
            WireUpRoundsList();
        }

        private void WireUpRoundsList()
        {
            roundsBinding.DataSource = rounds;
            roundDropDown.DataSource = roundsBinding;
        }

        private void WireUpMatchupsList()
        {

            matchupsBinding.DataSource = selectedMatchups;
            matchupListBox.DataSource = matchupsBinding;

            matchupListBox.DisplayMember = "DisplayName";
        }

        // Called initially when LoadRounds is called when the form is first created, and 
        // again when the round number is changed in the dropdown.
        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Loading the matches for the selected round and refreshing the matches listbox.
            LoadAllMatches();

            // Reading in the match data for the first (default selected) match in the ListBox, which happens when the round is
            // changed, meaning you don't have to click the match to get the display names to refresh.
            LoadMatch();
        }

        private void LoadAllMatches()
        {
            int round = (int)roundDropDown.SelectedItem;  

            foreach (List<MatchupModel> match in tournament.Rounds)
            {
                // Getting all the matches for the selected round
                if (match.First().MatchupRound == round)
                {
                    selectedMatchups = match;
                }
            }

            // Refreshing the matchups ListBox
            WireUpMatchupsList();
        }

        // Loading the names and scores for the selected matchup in the ListBox
        private void LoadMatch()
        {
          
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            for (int i = 0; i < m.Entries.Count; i++)
            {

                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreLabel.Text = m.Entries[0].Score.ToString();

                        // This is for if there is a bye in the first round.
                        // If no bye, then this will be overwritten with the second team below.

                        teamTwoName.Text = "Bye";
                        teamTwoScoreLabel.Text = "0";

                    } else
                    {
                        teamOneName.Text = "Not Yet Set";
                        teamOneScoreLabel.Text = "";
                    }
                }

                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreLabel.Text = m.Entries[1].Score.ToString();

                    }
                    else
                    {
                        teamTwoName.Text = "Not Yet Set";
                        teamTwoScoreLabel.Text = "";
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatch();
        }
    }
}
