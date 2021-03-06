﻿using Engine;
using System;
using System.Windows.Forms;

namespace Nayb_Game_UI
{
    public partial class TradingScreen : Form
    {
        public Player CurrentPlayer { get; set; }

        private Player _currentPlayer;

        public TradingScreen(Player player)
        {
            _currentPlayer = player;

            InitializeComponent();

            // Style, to display numberic column values
            DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
            rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Populate the datagrid for the player's inventory
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;

            // This hidden columns holds the item ID, so we know which item to sell
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30 + 8,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35 + 8,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Sell 1",
                UseColumnTextForButtonValue = true,
                Width = 50 + 5,
                DataPropertyName = "ItemID"
            });

            // Bind the player's inventory to the datagridview
            dgvMyItems.DataSource = _currentPlayer.Inventory;

            // When the user click on a row, call this function
            dgvMyItems.CellClick += dgvMyItems_CellClick;


            // Populate the datagrid for the vendor's inventory
            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;

            // This hidden column holds the item ID, so we know which item to sell
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30 + 8,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35 + 8,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Buy 1",
                UseColumnTextForButtonValue = true,
                Width = 50 + 5,
                DataPropertyName = "ItemID"
            });

            // Bind the vendor's inventory to the datagridview
            dgvVendorItems.DataSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;

            // When the use clicks on a row, call this function
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;
        }

        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // datagridview uses "zero-based" array/collection/list
            // So the 5th column is ColumnIndex = 4
            if (e.ColumnIndex == 4)
            {
                // This gets the ID value of the item, from the hidden 1st column
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;

                // Get the Item object for the selected item row
                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));

                if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                {
                    MessageBox.Show("You cannot sell the " + itemBeingSold.Name);
                }
                else
                {
                    // Remove one of these items from the player's inventory
                    _currentPlayer.RemoveItemFromInventory(itemBeingSold);
                    _currentPlayer.CurrentLocation.VendorWorkingHere.AddItemToInventory(itemBeingSold);

                    // Give the player the gold for the item being sold
                    _currentPlayer.Gold += itemBeingSold.Price;
                }
            }

        }

        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // The 5th column (ColumnIndex = 3) has the "Buy 1" button
            if (e.ColumnIndex == 4)
            {
                // Gets ID value of item, from the hidden 1st column
                var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;

                // Get the Item object for the selected item row
                Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));

                // Check if player has enough gold to buy
                if (_currentPlayer.Gold >= itemBeingBought.Price)
                {
                    // Add one of the items to the player's inventory
                    _currentPlayer.AddItemToInventory(itemBeingBought);

                    // Remove the gold to pay for item
                    _currentPlayer.Gold -= itemBeingBought.Price;

                    // Remove item from Vendor's inventory
                    _currentPlayer.CurrentLocation.VendorWorkingHere.RemoveItemFromInventory(itemBeingBought);
                }
                else
                {
                    MessageBox.Show("You do not have enough gold to buy the " + itemBeingBought.Name);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
