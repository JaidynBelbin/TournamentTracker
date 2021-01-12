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
            LoadAllMatches((int)roundDropDown.SelectedItem);

            // Reading in the match data for the first (default selected) match in the ListBox, which happens when the round is
            // changed, meaning you don't have to click the match to get the display names to refresh.
            if (selectedMatchups.Count > 0)
            {
                LoadSelectedMatch(); 
            }
        }

        private void LoadAllMatches(int round)
        {
            selectedMatchups = new List<MatchupModel>();

            foreach (List<MatchupModel> match in tournament.Rounds)
            {
                // Getting all the matches for the selected round
                if (match.First().MatchupRound == round)
                {
                    foreach (MatchupModel m in match)
                    {
                        if (m.Winner == null || !unplayedOnlyCheckbox.Checked)
                        {
                            selectedMatchups.Add(m);
                        }
                    }
                }
            }

            // Refreshing the matchups ListBox
            WireUpMatchupsList();

            DisplayMatchupInfo();
        }

        // Loading the names and scores for the selected matchup in the ListBox
        private void LoadSelectedMatch()
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            for (int i = 0; i < m.Entries.Count; i++)
            {

                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                        // This is for if there is a bye in the first round.
                        // If no bye, then this will be overwritten with the second team below.

                        teamTwoName.Text = "Bye";
                        teamTwoScoreValue.Text = "0";

                    } else
                    {
                        teamOneName.Text = "Not Yet Set";
                        teamOneScoreValue.Text = "";
                    }
                }

                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = m.Entries[1].Score.ToString();

                    }
                    else
                    {
                        teamTwoName.Text = "Not Yet Set";
                        teamTwoScoreValue.Text = "";
                    }
                }
            }
        }

        private void DisplayMatchupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);

            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;

            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;

            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedMatchups.Count > 0)
            {
                LoadSelectedMatch();
            }
        }

        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // TODO- Scores are being read in from the database, but checking the checkbox does nothing,
            // so the winners are not being found for some reason.

            LoadAllMatches((int)roundDropDown.SelectedItem);
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < m.Entries.Count; i++)
            {

                if (i == 0)
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);

                        if (scoreValid)
                        {
                            m.Entries[0].Score = teamOneScore;

                        } else
                        {
                            MessageBox.Show("Please enter a valid score for team one.");
                            return;
                        }
                    }
                }

                if (i == 1)
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

                        if (scoreValid)
                        {
                            m.Entries[1].Score = teamTwoScore;
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid score for team two.");
                            return;
                        }
                    }
                }
            }

            if (teamOneScore > teamTwoScore)
            {
                m.Winner = m.Entries[0].TeamCompeting;

            } else if (teamTwoScore > teamOneScore)
            {
                m.Winner = m.Entries[1].TeamCompeting;
               
            } else
            {
                MessageBox.Show("Tied games are not saved.");
            }

            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    foreach (MatchupEntryModel me in rm.Entries)
                    {
                        if (me.ParentMatchup != null)
                        {
                            if (me.ParentMatchup.ID == m.ID)
                            {
                                me.TeamCompeting = m.Winner;

                                GlobalConfig.Connection.UpdateMatchup(rm);
                            }
                        }
                    }
                }
            }

            // Refreshing the unplayed rounds
            LoadAllMatches((int)roundDropDown.SelectedItem);

            // Saving the scores and stuff
            GlobalConfig.Connection.UpdateMatchup(m);
        }
    }
}
