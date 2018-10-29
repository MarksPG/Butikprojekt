using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;


namespace Butik_PGCJ
{
    class Guitar
    {
        public string ItemName;
        public int ItemPrice;
        public string ItemPic;
        public string ItemDescr;
    }

    class Discount
    {
        public string DiscountName;
        public int DiscountValue;

        public static List<Discount> ReadDiscountFile()
        {
            string[] lines = File.ReadAllLines("Discounts.csv");
            List<Discount> discountItem = new List<Discount> { };
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Discount d = new Discount
                {
                    DiscountName = values[0],
                    DiscountValue = int.Parse(values[1])
                };
                discountItem.Add(d);
            }
            return discountItem;
        }
    }

    //class Store
    //{
    //}

    

    class MyForm : Form
    {
        Button doCheckout = new Button();
        Button addItemToCart = new Button();
        Button removeItemFromCart = new Button();
        Button addDiscount = new Button();
        Button saveCart = new Button();
        Button loadCart = new Button();
        Button clearCart = new Button();

        public static double discountGlobalValue = 0;

        public static Dictionary<Guitar, int> shoppingCart = new Dictionary<Guitar, int>();

        Label itemListLabel = new Label();
        Label itemDescriptionLabel = new Label();
        Label itemCartLabel = new Label();
        Label sumLabel = new Label();
        Label priceLabel = new Label();
        Label discountLabel = new Label();

        List<Guitar> shopItems = new List<Guitar>();

        ListBox itemList;

        ListView itemCart = new ListView();

        ListViewItem item;

        PictureBox itemPicture = new PictureBox();

        TableLayoutPanel outline = new TableLayoutPanel();
        TableLayoutPanel outlineBelowItemCart = new TableLayoutPanel();
        TableLayoutPanel outlinePriceInformation = new TableLayoutPanel();
        TableLayoutPanel outlineSaveAndLoad = new TableLayoutPanel();

        TextBox itemDescriptionTextbox = new TextBox();
        TextBox itemDescriptionAdditionalTextbox = new TextBox();
        TextBox discountTextbox = new TextBox();
        static TextBox sumTextbox = new TextBox();
        TextBox priceTextbox = new TextBox();

        public MyForm()
        {
            //Talar om storleken på winform vid uppstart
            ClientSize = new Size(850, 550);
            Icon = new Icon(@"Pictures\guitar_icon.ico");
            Text = "PGCJ Gitarraffär - plocka dina strängar online";

            // Outlines
            outline = CreateOutline(8, 4);
            Controls.Add(outline);

            outlineBelowItemCart = CreateOutline(1, 3);
            outline.Controls.Add(outlineBelowItemCart, 3, 5);

            outlinePriceInformation = CreateOutline(1, 3);
            outline.Controls.Add(outlinePriceInformation, 1, 2);
            outline.SetColumnSpan(outlinePriceInformation, 2);

            outlineSaveAndLoad = CreateOutline(1, 2);
            outline.Controls.Add(outlineSaveAndLoad, 3, 6);

            // Create rows & columns for outlines



            // Labels
            itemListLabel = CreateLabel(AnchorStyles.None, true, "Utbud");
            outline.Controls.Add(itemListLabel, 0, 0);

            itemDescriptionLabel = CreateLabel(AnchorStyles.None, true, "Beskrivning av vara");
            outline.Controls.Add(itemDescriptionLabel, 1, 0);
            outline.SetColumnSpan(itemDescriptionLabel, 2);

            priceLabel = CreateLabel(AnchorStyles.Left, true, "Pris:");
            outlinePriceInformation.Controls.Add(priceLabel, 0, 0);
            
            discountLabel = CreateLabel(AnchorStyles.Top | AnchorStyles.Left, true, "");
            outline.Controls.Add(discountLabel, 1, 6);
            outline.SetColumnSpan(discountLabel, 2);

            sumLabel = CreateLabel(AnchorStyles.Left, true, "Summa:");
            outlineBelowItemCart.Controls.Add(sumLabel, 0, 0);

            itemCartLabel = CreateLabel(AnchorStyles.None, true, "Varukorg");
            outline.Controls.Add(itemCartLabel, 3, 0);

            // Buttons
            addItemToCart = CreateButton(AnchorStyles.None, "Lägg till vara -->");
            outline.Controls.Add(addItemToCart, 1, 3);
            outline.SetColumnSpan(addItemToCart, 2);
            addItemToCart.Click += ItemCartAddClicked;

            removeItemFromCart = CreateButton(AnchorStyles.None, "<-- Ta bort vara");
            outline.Controls.Add(removeItemFromCart, 1, 4);
            outline.SetColumnSpan(removeItemFromCart, 2);
            removeItemFromCart.Click += ItemCartRemClicked;

            saveCart = CreateButton(AnchorStyles.None, "Spara varukorg");
            outlineSaveAndLoad.Controls.Add(saveCart, 0, 0);
            saveCart.Click += saveAllItemsFromCart;

            loadCart = CreateButton(AnchorStyles.None, "Ladda varukorg");
            outlineSaveAndLoad.Controls.Add(loadCart, 1, 0);
            loadCart.Click += loadSavedCart;

            doCheckout = CreateButton(AnchorStyles.None, "Checkout");
            outlineBelowItemCart.Controls.Add(doCheckout, 2, 0);
            doCheckout.Click += checkoutButtonClicked;

            addDiscount = CreateButton(AnchorStyles.Bottom | AnchorStyles.Left, "Applicera rabatt");
            outline.Controls.Add(addDiscount, 2, 5);
            addDiscount.Click += discountButtonClicked;

            clearCart = new Button()
            {
                Text = "Töm varukorg",
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Dock = DockStyle.Fill
            };
            outline.Controls.Add(clearCart, 3, 7);
            clearCart.Click += removeAllItemsFromCart;

            //Lista över tillgänliga varor i shoppen
            itemList = new ListBox()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
            };
            outline.Controls.Add(itemList);
            outline.SetRowSpan(itemList, 4);
            itemList.SelectedIndexChanged += ItemListBoxClicked;
            
            //Picturebox
            itemPicture = new PictureBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None,
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            outline.Controls.Add(itemPicture, 1, 1);

            //Textbox med huvudinformation om varan
            itemDescriptionTextbox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White
            };
            outline.Controls.Add(itemDescriptionTextbox, 2, 1);

            //Addering av priceTextbox
            priceTextbox = new TextBox()
            {
                Anchor = AnchorStyles.Left,
                ReadOnly = true
            };
            outlinePriceInformation.Controls.Add(priceTextbox, 1, 0);

            discountTextbox = new TextBox()
            {
                Text = "Skriv in rabattkod här...",
                Dock = DockStyle.Bottom,
            };
            outline.Controls.Add(discountTextbox, 1, 5);
            discountTextbox.Click += discountTextBoxClicked;

            sumTextbox = new TextBox()
            {
                Anchor = AnchorStyles.Left,
                ReadOnly = true,
                Enabled = false
            };
            outlineBelowItemCart.Controls.Add(sumTextbox, 1, 0);

            

            

            //ListView där adderade varor visas.
            itemCart = new ListView()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false
            };
            outline.SetRowSpan(itemCart, 4);
            outline.Controls.Add(itemCart, 3, 1);

            itemCart.Columns.Add("Vara");
            itemCart.Columns.Add("Antal");
            itemCart.Columns.Add("Pris");
            
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            outlineSaveAndLoad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            outlineSaveAndLoad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            outline.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            for (int i = 0; i < 6; i++) { outline.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); }

            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            shopItems = ReadVendorFile();
        }

        public List<Guitar> ReadVendorFile()
        {
            //Inläsning från csv-fil "VendingSupply.csv" och addering till Lista fylld
            //med objekt av typen klassen Guitar
            string[] lines = File.ReadAllLines("VendingSupply.csv");
            shopItems = new List<Guitar> { };
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Guitar g = new Guitar
                {
                    ItemName = values[0],
                    ItemPrice = int.Parse(values[1]),
                    ItemPic = values[2],
                    ItemDescr = values[3]
                };
                itemList.Items.Add(g.ItemName);
                shopItems.Add(g);
            }
            return shopItems;
        }

        private void saveAllItemsFromCart(object sender, EventArgs e)
        {
            string path = @"C:\Windows\Temp\savedCart.txt";
            File.WriteAllLines(path, shoppingCart.Select(kvp => string.Format("{0},{1},{2},{3},{4}", kvp.Key.ItemName, kvp.Key.ItemPrice, kvp.Key.ItemPic, kvp.Key.ItemDescr, kvp.Value)));
        }

        private void removeAllItemsFromCart(object sender, EventArgs e)
        {
            itemCart.Items.Clear();
            shoppingCart.Clear();
            sumTextbox.Text = "";
        }

        private void loadSavedCart(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(@"C:\Windows\Temp\savedCart.txt");
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Guitar g = new Guitar
                {
                    ItemName = values[0],
                    ItemPrice = int.Parse(values[1]),
                    ItemPic = values[2],
                    ItemDescr = values[3]
                };

                
                var index = shoppingCart.FirstOrDefault(x => x.Key.ItemName == g.ItemName);
                if (shoppingCart.ContainsKey(g) || index.Key != null)
                {
                    shoppingCart[index.Key]++;
                }
                else
                {
                    shoppingCart.Add(g, 1);
                }
                

                //shoppingCart.Add(g, int.Parse(values[4]));
            }
            UpdateListView(shoppingCart);
            UpdateSum();
            loadCart.Enabled = false;
        }

        private void discountTextBoxClicked(object sender, EventArgs e)
        {
            discountTextbox.Text = string.Empty;
        }

        private void discountButtonClicked(object sender, EventArgs e)
        {
            List<Discount> discountItem = Discount.ReadDiscountFile();
            string enteredCode = discountTextbox.Text;

            if (discountItem.Any(d => d.DiscountName == enteredCode))
            {
                int actualDiscount = discountItem.Where(d => d.DiscountName == enteredCode).Select(d => d.DiscountValue).Single();
                discountLabel.Text = "Grattis, koden är giltig och ger dig " + actualDiscount + "% på allt du köper!";
                discountGlobalValue = actualDiscount;
                addDiscount.Enabled = false;
                discountTextbox.Text = "Du har redan angivit en rabattkod!";
                discountTextbox.ReadOnly = true;                                // Kanske onödig..
                discountTextbox.Enabled = false;
            }
            else
            {
                discountLabel.Text = "Tyvärr, koden är inte giltig!";
            }
            UpdateSum();
        }

        private void ItemListBoxClicked(object sender, EventArgs e)
        {
            Guitar g = shopItems[itemList.SelectedIndex];
            itemDescriptionTextbox.Text = g.ItemDescr;
            priceTextbox.Text = g.ItemPrice.ToString() + " kr";
            itemPicture.Image = Image.FromFile(@"Pictures\" + g.ItemPic);
        }

        private void ItemCartAddClicked(object sender, EventArgs e)
        {
            try
            {
                Guitar g = shopItems[itemList.SelectedIndex];
                var index = shoppingCart.FirstOrDefault(x => x.Key.ItemName == g.ItemName);
                if (shoppingCart.ContainsKey(g) || index.Key != null)
                {
                    shoppingCart[index.Key]++;
                }
                else
                {
                    shoppingCart.Add(g, 1);
                }
                UpdateListView(shoppingCart);
                UpdateSum();
            }
            catch
            {
                MessageBox.Show("Välj en vara att lägga till");
            }
        }

        private void ItemCartRemClicked(object sender, EventArgs e)
        {
            try
            {
                string itemCheckTag = itemCart.SelectedItems[0].Tag.ToString();
                var itemToRemove = shoppingCart.Where(i => i.Key.ItemName == itemCheckTag).ToArray();
                foreach (var item in itemToRemove)
                {
                    if (item.Value > 1)
                    {
                        shoppingCart[item.Key] = item.Value - 1;
                        UpdateListView(shoppingCart);
                    }
                    else
                    {
                        shoppingCart.Remove(item.Key);
                        itemCart.Items.Remove(itemCart.SelectedItems[0]);
                    }
                }
                UpdateSum();
            }
            catch
            {
                MessageBox.Show("Välj en vara att ta bort");
            }
        }

        private void UpdateListView(Dictionary<Guitar, int> shoppingCart)
        {
            itemCart.Items.Clear();

            foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
            {
                item = new ListViewItem(pair.Key.ItemName);
                item.SubItems.Add(pair.Value.ToString());
                item.SubItems.Add(pair.Key.ItemPrice.ToString());
                item.Tag = pair.Key.ItemName;
                itemCart.Items.Add(item);
            }
        }

        public static double UpdateSum()
        {
            sumTextbox.Text = String.Empty;
            double sumTotal = 0;

            if (discountGlobalValue > 0)
            {
                foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
                {
                    sumTotal += (pair.Key.ItemPrice * pair.Value) * (1 - discountGlobalValue / 100);
                }
            }
            else
            {
                foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
                {
                    sumTotal += pair.Key.ItemPrice * pair.Value;
                }
            }
            sumTextbox.Text = sumTotal.ToString() + " kr";
            return sumTotal;
        }

        private void checkoutButtonClicked(object sender, EventArgs e)
        {
            var myForm = new MyForm2();
            myForm.StartPosition = FormStartPosition.CenterScreen;
            myForm.BackColor = Color.White;
            myForm.Show();
        }

        private static TableLayoutPanel CreateOutline(int row, int column)
        {
            return new TableLayoutPanel
            {
                RowCount = row,
                ColumnCount = column,
                Dock = DockStyle.Fill
            };
        }

        private static Label CreateLabel(AnchorStyles anchor, bool value, string name)
        {
            return new Label
            {
                Anchor = anchor,
                AutoSize = value,
                Text = name,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };
        }

        private static Button CreateButton(AnchorStyles anchor, string name)
        {
            return new Button
            {
                Anchor = anchor,
                AutoSize = true,
                Text = name,
            };
        }

    }

    class MyForm2 : Form
    {
        public MyForm2()
        {
            Icon = new Icon(@"Pictures\guitar_icon.ico");
            Text = "PGCJ Gitarraffär - Tack för att du handlat hos oss.";
            Dictionary<Guitar, int> shoppingCart = MyForm.shoppingCart;
            double discountGlobalValue = MyForm.discountGlobalValue;

            ClientSize = new Size(400, 300);

            TableLayoutPanel outlineReceipt = new TableLayoutPanel
            {
                RowCount = 6,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            Controls.Add(outlineReceipt);

            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            //outlineReceipt.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            Label receipt = new Label()
            {
                Text = "Kvitto",
                Font = new Font("Arial", 14, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(receipt, 0, 0);

            Label itemName = new Label()
            {
                Anchor = AnchorStyles.Left,
                Text = "Artikel",
                Font = new Font("Arial", 12, FontStyle.Bold),
            };
            outlineReceipt.Controls.Add(itemName, 0, 2);

            Label quantity = new Label()
            {
                Anchor = AnchorStyles.Left,
                Text = "Antal",
                Font = new Font("Arial", 12, FontStyle.Bold),

            };
            outlineReceipt.Controls.Add(quantity, 1, 2);

            Label piecePrice = new Label()
            {
                Anchor = AnchorStyles.Left,
                Text = "á-Pris",
                Font = new Font("Arial", 12, FontStyle.Bold),
            };
            outlineReceipt.Controls.Add(piecePrice, 2, 2);

            DataGridView dtgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersVisible = false,
                BackgroundColor = Color.White,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
            };
            outlineReceipt.Controls.Add(dtgv, 0, 3);
            outlineReceipt.SetColumnSpan(dtgv, 3);

            dtgv.DefaultCellStyle.SelectionBackColor = dtgv.DefaultCellStyle.BackColor;
            dtgv.DefaultCellStyle.SelectionForeColor = dtgv.DefaultCellStyle.ForeColor;

            DataGridViewTextBoxColumn itemNameGrid = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 51,
                ReadOnly = true,
            };

            DataGridViewTextBoxColumn quantityGrid = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 25,
                ReadOnly = true,
            };

            DataGridViewTextBoxColumn priceGrid = new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 23,
                ReadOnly = true,
            };

            dtgv.Columns.AddRange(new DataGridViewColumn[]
            {
                itemNameGrid, quantityGrid, priceGrid
            });

            foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
            {
                dtgv.Rows.Add(pair.Key.ItemName, pair.Value, pair.Key.ItemPrice);
            }

            Label totalPriceLabelText = new Label()
            {
                Text = "Totalpris",
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
            };
            outlineReceipt.Controls.Add(totalPriceLabelText, 1, 4);

            Label totalPriceLabel = new Label()
            {
                Text = MyForm.UpdateSum().ToString() + " kr",
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
            };
            outlineReceipt.Controls.Add(totalPriceLabel, 2, 4);
        }
    }
}