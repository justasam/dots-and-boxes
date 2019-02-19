using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        Random r = new Random();
        Color[] players =
        {
            Color.Blue,
            Color.Red
        };
        int currentPlayer;
        int trackMoves = 0;
        int size = 4;
        int[] score = { 0, 0 };

        Label[] scoreInfo =
        {
            // blue score
            new Label()
            {
                ForeColor = Color.Blue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            },
            // red score
            new Label()
            {
                ForeColor = Color.Red,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            },
            // current turn
            new Label()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            }
        };
        Button[,] lineBtns;

        public Form1()
        {
            InitializeComponent();
            InitStartScreen();
        }

        // gets called once form loads, sets icon and text
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Dots and Boxes";
            Icon icon = Icon.ExtractAssociatedIcon("Resources/DotsAndBoxes.ico");
            this.Icon = icon;
        }

        // centers element in form
        void CenterElement(Control element, bool horizontal = true, bool vertical = true)
        {
            if (horizontal)
                element.Left = (this.Width - element.Width) / 2;
            if (vertical)
                element.Top = (this.Height - element.Height) / 2;
        }

        // handlers
        void startClick(object sender, EventArgs e)
        {
            Controls["startScreen"].Visible = false;
            
            InitGameScreen();
        }

        void restartClick(object sender, EventArgs e)
        {
            score[0] = 0;
            score[1] = 0;
            trackMoves = 0;
            Controls.Remove(Controls["infoGrid"]);
            Controls.Remove(Controls["gameGrid"]);
            Controls["startScreen"].Visible = true;
        }

        void lineBtnClick(object sender, EventArgs e)
        {
            Button clickedBtn = (Button)sender;
            trackMoves++;
            clickedBtn.Enabled = false;
            clickedBtn.BackColor = Color.DarkSlateGray;

            int y = -1, x = -1;

            // detects which line button is clicked and checks whether it completes a square
            for (int i = 0; i < size * 2 - 1; i++)
            {
                for (int c = 0; c < size; c++)
                {
                    if (lineBtns[i,c] != null && lineBtns[i,c].Equals(clickedBtn))
                    {
                        y = i;
                        x = c;
                        break;
                    }
                }
                if (y != -1) break;
            }
            checkIfSquare(x, y);
        }

        void sizeSelectChange(object sender, EventArgs e)
        {
            size = (int)((NumericUpDown)sender).Value;
        }

        void OnMouseEnterLineBtn(object sender, EventArgs e)
        {
            if (((Button)sender).Enabled)
                ((Button)sender).BackColor = currentPlayer == 0 ? Color.LightBlue : Color.LightSalmon;
        }

        void OnMouseLeaveLineBtn(object sender, EventArgs e)
        {
            if (((Button)sender).Enabled)
                ((Button)sender).BackColor = Color.Gray;
        }

        // checks if there is a square associated with the clicked button,
        // if so, updates score and gives another turn to that player
        void checkIfSquare(int x, int y)
        {
            int scoreBeforeMove = score[currentPlayer];

            // check top and bottom
            if (y % 2 == 0)
            {

                if (y != 0 && (isClicked(x, y - 2) && isClicked(x, y - 1) && isClicked(x + 1, y - 1)))
                {
                    setMultiColor(players[currentPlayer], new int[] { x, y, x, y - 2, x, y - 1, x + 1, y - 1 });
                    score[currentPlayer]++;
                }
                if (y != size * 2 - 2 && (isClicked(x, y + 2) && isClicked(x, y + 1) && isClicked(x + 1, y + 1)))
                {
                    setMultiColor(players[currentPlayer], new int[] { x, y, x, y + 2, x, y + 1, x + 1, y + 1});
                    score[currentPlayer]++;
                }
            }
            // check left and right
            else
            {
                if (x != 0 && (isClicked(x - 1, y - 1) && isClicked(x - 1, y + 1) && isClicked(x - 1, y)))
                {
                    setMultiColor(players[currentPlayer], new int[] { x, y, x - 1, y - 1, x - 1, y + 1, x - 1, y });
                    score[currentPlayer]++;
                }
                if (x != size - 1 && (isClicked(x, y - 1) && isClicked(x, y + 1) && isClicked(x + 1, y)))
                {
                    setMultiColor(players[currentPlayer], new int[] { x, y, x, y - 1, x, y + 1, x + 1, y });
                    score[currentPlayer]++;
                }
            }

            if (scoreBeforeMove == score[currentPlayer]) changePlayer();
            else
            {
                scoreInfo[currentPlayer].Text = score[currentPlayer] + "";
                scoreInfo[2].Text = (currentPlayer == 0 ? "Blue" : "Red") + " Again!";
            }
            if (trackMoves == 2 * size * (size - 1))
            {
                if (score[0] == score[1])
                {
                    System.Windows.Forms.MessageBox.Show("Woah, it's a tie!");
                    return;
                }
                
                System.Windows.Forms.MessageBox.Show("Good game! The winner is " + (score[0] > score[1] ? "Blue" : "Red"));
            }
        }

        void setMultiColor(Color color, int[] coords)
        {
            if (coords.Length != 8) return;
            for (int i = 0; i < 8; i+=2)
            {
                lineBtns[coords[i + 1], coords[i]].BackColor = color;
            }
        }

        bool isClicked(int x, int y)
        {
            return !lineBtns[y, x].Enabled;
        }

        void changePlayer()
        {
            currentPlayer = currentPlayer == 0 ? 1 : 0;
            scoreInfo[2].Text = currentPlayer == 0 ? "Blue" : "Red";
            scoreInfo[2].ForeColor = players[currentPlayer];
        }

        // initalizes and renders start screen
        void InitStartScreen()
        {
            TableLayoutPanel startScreen = new TableLayoutPanel()
            {
                RowCount = 6,
                ColumnCount = 3,
                AutoSize = true,
                BackColor = Color.LightGray,
                Name = "startScreen",
                Padding = new Padding(10),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset
            };


            startScreen.ColumnStyles.Clear();
            for (int i = 0; i < startScreen.ColumnCount; i++)
            {
                ColumnStyle cs = new ColumnStyle(SizeType.Absolute, 100);

                startScreen.ColumnStyles.Add(cs);
            }

            Label gameName = new Label()
            {
                Text = "Dots and Boxes",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label title = new Label()
            {
                Text = "INFO:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.CadetBlue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            String info = "This is a 2 player game (red vs blue).\n" +
                "The goal of this game is to join more squares than the other player.\n" +
                "You join squares by pressing buttons next to circles (they symbolize square edges).\n" +
                "Each player gets one turn, + one bonus turn once they join a square.\n";

            String infoImportant = "Choose who is blue and who is red before pressing the button!\n" +
                "Starting color will be randomly generated.";

            Label howToPlay = new Label()
            {
                Text = info,
                Font = new Font("Arial", 8, FontStyle.Italic),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            Label howToPlayImportant = new Label()
            {
                Text = infoImportant,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            Button startBtn = new Button()
            {
                Text = "Start",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            startBtn.Click += new EventHandler(this.startClick);

            Label sizeSelectLabel = new Label()
            {
                Text = "Select grid size:",
                Enabled = false,
                Font = new Font("Arial", 8, FontStyle.Italic),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true
            };

            NumericUpDown sizeSelect = new NumericUpDown()
            {
                Value = 4,
                Name = "sizeSelect",
                Minimum = 2,
                Maximum = 12
            };

            sizeSelect.ValueChanged += sizeSelectChange;

            startScreen.Controls.Add(gameName, 0, 0);
            startScreen.SetColumnSpan(gameName, 3);
            startScreen.Controls.Add(title, 1, 1);
            startScreen.Controls.Add(howToPlay, 0, 2);
            startScreen.SetColumnSpan(howToPlay, 3);
            startScreen.Controls.Add(howToPlayImportant, 0, 3);
            startScreen.SetColumnSpan(howToPlayImportant, 3);
            startScreen.Controls.Add(sizeSelectLabel, 0, 4);
            startScreen.Controls.Add(sizeSelect, 1, 4);
            startScreen.Controls.Add(startBtn, 1, 5);

            Controls.Add(startScreen);
            CenterElement(startScreen);

        }

        // initalizes and renders game screen
        void InitGameScreen()
        {
            currentPlayer = r.Next(2);
            InitGameGrid();
            InitInfoGrid();
        }
        
        // setups game grid, intializes and renders it
        void InitGameGrid()
        {
            const int GRID_SQUARE_SIZE = 40, GRID_DOT_SIZE = 15;
            TableLayoutPanel gameGrid = new TableLayoutPanel()
            {
                RowCount = size * 2 - 1,
                ColumnCount = size * 2 - 1,
                BackColor = Color.LightGray,
                Width = GRID_SQUARE_SIZE * (size - 1) + GRID_DOT_SIZE * size,
                Height = GRID_SQUARE_SIZE * (size - 1) + GRID_DOT_SIZE * size,
                Name = "gameGrid"
            };

            gameGrid.ColumnStyles.Clear();
            gameGrid.RowStyles.Clear();

            // since same number of rows and cols, only need one loop
            for (int i = 0; i < gameGrid.ColumnCount; i++)
            {
                ColumnStyle cs = new ColumnStyle(SizeType.Absolute, i % 2 == 0 ? GRID_DOT_SIZE : GRID_SQUARE_SIZE);
                RowStyle rs = new RowStyle(SizeType.Absolute, i % 2 == 0 ? GRID_DOT_SIZE : GRID_SQUARE_SIZE);

                gameGrid.ColumnStyles.Add(cs);
                gameGrid.RowStyles.Add(rs);
            }

            lineBtns = new Button[size * 2 - 1, size];

            // offset which allows blank spaces in table
            int btnOffset;

            // creates, renders and adds handler to buttons
            for (int i = 0; i < size * 2 - 1; i++)
            {
                btnOffset = i % 2 == 0 ? 1 : 0;
                for (int x = 0; x < size; x++)
                {
                    // on every other line there has to be one less button
                    if (x == size - 1 && i % 2 == 0)
                    {
                        continue;
                    }
                    lineBtns[i, x] = new Button()
                    {
                        BackColor = Color.Gray,
                        Dock = DockStyle.Fill,
                        Padding = new Padding(0),
                        Margin = new Padding(0),
                        Text = "",
                        FlatStyle = FlatStyle.Flat,
                        TabStop = false
                    };
                    // https://stackoverflow.com/questions/9399215/c-sharp-winforms-custom-button-unwanted-border-when-form-unselected
                    lineBtns[i, x].FlatAppearance.BorderSize = 0;

                    lineBtns[i, x].Click += lineBtnClick;
                    lineBtns[i, x].MouseEnter += OnMouseEnterLineBtn;
                    lineBtns[i, x].MouseLeave += OnMouseLeaveLineBtn;
                    gameGrid.Controls.Add(lineBtns[i, x], btnOffset + x, i);
                    btnOffset += 1;
                }
            }

            Controls.Add(gameGrid);
            CenterElement(gameGrid);
            gameGrid.Top = Math.Max(gameGrid.Top, 150);
        }

        // initalizes and renders info grid which shows score, current player, restart button
        void InitInfoGrid()
        {
            TableLayoutPanel infoGrid = new TableLayoutPanel()
            {
                RowCount = 3,
                ColumnCount = 3,
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGray,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetPartial,
                Name = "infoGrid"
            };

            infoGrid.Controls.Add(new Label()
            {
                Text = "Blue:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            }, 0, 0);
            infoGrid.Controls.Add(new Label()
            {
                Text = "Turn:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            }, 1, 0);
            infoGrid.Controls.Add(new Label()
            {
                Text = "Red:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            }, 2, 0);

            Button restartBtn = new Button()
            {
                BackColor = Color.LightGray,
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Text = "Restart",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.CadetBlue,
                TabStop = false
            };
            restartBtn.FlatAppearance.BorderSize = 0;
            restartBtn.Click += restartClick;

            infoGrid.Controls.Add(restartBtn, 1, 2);

            scoreInfo[0].Text = score[0] + "";
            scoreInfo[1].Text = score[1] + "";
            scoreInfo[2].ForeColor = players[currentPlayer];
            scoreInfo[2].Text = currentPlayer == 1 ? "Red" : "Blue";

            infoGrid.Controls.Add(scoreInfo[0], 0, 1);
            infoGrid.Controls.Add(scoreInfo[1], 2, 1);
            infoGrid.Controls.Add(scoreInfo[2], 1, 1);

            Controls.Add(infoGrid);
            CenterElement(infoGrid, true, false);
        }
    }
}