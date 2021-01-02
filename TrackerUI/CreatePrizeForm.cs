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
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        // Passing in *anything* that implements IPrizeRequester into the constructor of this form
        // in order to hand back the completed PrizeModel.
        IPrizeRequester callingForm;
        public CreatePrizeForm(IPrizeRequester caller)
        {
            InitializeComponent();

            callingForm = caller;
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                // Calling the overloaded constructor to make a new PrizeModel with the
                // validated input.
                PrizeModel model = new PrizeModel(
                    placeNameValue.Text, 
                    placeNumberValue.Text, 
                    prizeAmountValue.Text, 
                    prizePercentageValue.Text);

                // Creates a new Connection and calls the necessary CreatePrize method
                // from either SQLConnector or TextConnector
                GlobalConfig.Connection.CreatePrize(model);

                // Passing the completed model back to the PrizeComplete method in the calling form.
                callingForm.PrizeComplete(model);

                this.Close();

            } else
            {
                MessageBox.Show("One or more of the fields contains invalid information. Please check and try again.");
            }
        }

        // Validating the information in the text fields
        private bool ValidateForm()
        {
            bool output = true;

            // Stores the validated place number from the PlaceNumberValue textbox.
            int placeNumber = 0;

            // Trying to parse the input to an int and returning it in the placeNumber variable.
            bool placeNumberValidNumber = int.TryParse(placeNumberValue.Text, out placeNumber);
             
            // Dealing invalid inputs.
            if (!placeNumberValidNumber)
            {
                output = false;
            }

            if (placeNumber < 1)
            {
                output = false;
            }

            if(placeNameValue.Text.Length == 0)
            {
                output = false;
            }

            decimal prizeAmount = 0;
            double prizePercentage = 0;

            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out prizePercentage);

            if (!prizeAmountValid || !prizePercentageValid)
            {
                output = false;
            }

            if (prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }

            if (prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }
    }
}
