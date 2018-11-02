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

        public static List<Guitar> ReadVendorFile()
        {
            string[] lines = File.ReadAllLines("Guitars.csv");
            List<Guitar> shopItems = new List<Guitar> { };
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
                MyForm.itemList.Items.Add(g.ItemName);
                shopItems.Add(g);
            }
            return shopItems;
        }
    }

    class Accessory
    {
        public string ItemName;
        public int ItemPrice;
        public string ItemPic;
        public string ItemDescr;

        public static List<Accessory> ReadVendorFile()
        {
            string[] lines = File.ReadAllLines("Accessories.csv");
            List<Accessory> shopItemsAccessories = new List<Accessory> { };
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Accessory a = new Accessory
                {
                    ItemName = values[0],
                    ItemPrice = int.Parse(values[1]),
                    ItemPic = values[2],
                    ItemDescr = values[3]
                };
                MyForm.itemList.Items.Add(a.ItemName);
                shopItemsAccessories.Add(a);
            }
            return shopItemsAccessories;
        }
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

    class MyForm : Form
    {
        // Global initializing
        public static double discountGlobalValue = 0;
        public static Dictionary<Guitar, int> shoppingCart = new Dictionary<Guitar, int>();
        public static ListBox itemList;

        // Non global initializing
        Button doCheckout = new Button();
        Button addItemToCart = new Button();
        Button removeItemFromCart = new Button();
        Button addDiscount = new Button();
        Button saveCart = new Button();
        Button loadCart = new Button();
        Button clearCart = new Button();
        Button loadGuitars = new Button();
        Button loadAccessories = new Button();

        Label itemDescriptionLabel = new Label();
        Label itemCartLabel = new Label();
        Label actualPriceLabel = new Label();
        Label discountLabel = new Label();
        static Label sumLabel = new Label();

        List<Guitar> shopItems = new List<Guitar>();

        ListView itemCart = new ListView();

        ListViewItem item;

        PictureBox itemPicture = new PictureBox();

        TableLayoutPanel outline = new TableLayoutPanel();
        TableLayoutPanel outlineBelowItemCart = new TableLayoutPanel();
        TableLayoutPanel outlineBelowShopItems = new TableLayoutPanel();
        TableLayoutPanel outlinePriceInformation = new TableLayoutPanel();
        TableLayoutPanel outlineSaveAndLoad = new TableLayoutPanel();

        TextBox itemDescriptionTextbox = new TextBox();
        TextBox discountTextbox = new TextBox();

        public MyForm()
        {
            // Sets clientsize
            ClientSize = new Size(780, 550);

            // Icon for store
            Icon = new Icon(@"Pictures\guitar_icon.ico");

            // Name of store
            Text = "PGCJ Gitarraffär - plocka dina strängar online";
            
            // Background color of MyForm
            BackColor = SystemColors.InactiveBorder;

            // Outlines
            outline = CreateOutline(8, 4);
            Controls.Add(outline);

            outlineBelowItemCart = CreateOutline(1, 3);
            outline.Controls.Add(outlineBelowItemCart, 3, 5);

            outlineBelowShopItems = CreateOutline(1, 2);
            outline.Controls.Add(outlineBelowShopItems, 0, 5);

            outlinePriceInformation = CreateOutline(1, 3);
            outline.Controls.Add(outlinePriceInformation, 1, 2);
            outline.SetColumnSpan(outlinePriceInformation, 2);

            outlineSaveAndLoad = CreateOutline(1, 2);
            outline.Controls.Add(outlineSaveAndLoad, 3, 6);

            // Rows & Columns for outlines
            outline.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            outline.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            for (int i = 0; i < 5; i++) { outline.RowStyles.Add(new RowStyle(SizeType.Percent, 10)); }

            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            outline.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            outlineBelowItemCart.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 67));

            for (int i = 0; i < 2; i++) { outlineSaveAndLoad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); }

            // Labels
            outline.Controls.Add(CreateLabel(AnchorStyles.None, true, "Utbud"), 0, 0); // itemListLabel

            itemDescriptionLabel = CreateLabel(AnchorStyles.None, true, "Beskrivning av vara");
            outline.Controls.Add(itemDescriptionLabel, 1, 0);
            outline.SetColumnSpan(itemDescriptionLabel, 2);

            outlinePriceInformation.Controls.Add(CreateLabel(AnchorStyles.Right, true, "Pris:"), 0, 0); // priceLabel

            actualPriceLabel = CreateLabel(AnchorStyles.Left, true, "");
            outlinePriceInformation.Controls.Add(actualPriceLabel, 1, 0);

            discountLabel = CreateLabel(AnchorStyles.Top | AnchorStyles.Left, true, "");
            outline.Controls.Add(discountLabel, 1, 6);
            outline.SetColumnSpan(discountLabel, 2);

            outlineBelowItemCart.Controls.Add(CreateLabel(AnchorStyles.Left, true, "Summa:"), 0, 0); // sumLabel

            itemCartLabel = CreateLabel(AnchorStyles.None, true, "Varukorg");
            outline.Controls.Add(itemCartLabel, 3, 0);

            sumLabel = new Label
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Font = new Font("Arial", 11)
            };
            outlineBelowItemCart.Controls.Add(sumLabel, 1, 0);

            // Buttons
            addItemToCart = new Button
            {
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Text = "Lägg till vara -->",
                Size = new Size(150, 40),
                Font = new Font("Arial", 9),
                BackColor = Color.DarkKhaki
            };
            outline.Controls.Add(addItemToCart, 1, 3);
            outline.SetColumnSpan(addItemToCart, 2);
            addItemToCart.Click += ItemCartAddClicked;

            removeItemFromCart = new Button
            {
                Anchor = AnchorStyles.Top,
                AutoSize = true,
                Text = "<-- Ta bort vara",
                Size = new Size(150, 40),
                Font = new Font("Arial", 9),
                BackColor = Color.DarkKhaki
            };
            outline.Controls.Add(removeItemFromCart, 1, 4);
            outline.SetColumnSpan(removeItemFromCart, 2);
            removeItemFromCart.Click += ItemCartRemClicked;

            loadGuitars = CreateButton(AnchorStyles.None, "Visa gitarrer");
            outlineBelowShopItems.Controls.Add(loadGuitars, 0, 0);
            loadGuitars.Click += loadGuitarToItemListView;

            loadAccessories = CreateButton(AnchorStyles.None, "Visa tillbehör");
            outlineBelowShopItems.Controls.Add(loadAccessories, 1, 0);
            loadAccessories.Click += loadAccessoryToItemListView;


            saveCart = CreateButton(AnchorStyles.None, "Spara varukorg");
            outlineSaveAndLoad.Controls.Add(saveCart, 0, 0);
            saveCart.Click += saveAllItemsFromCart;

            loadCart = CreateButton(AnchorStyles.None, "Ladda varukorg");
            outlineSaveAndLoad.Controls.Add(loadCart, 1, 0);
            loadCart.Click += loadSavedCart;

            doCheckout = CreateButton(AnchorStyles.None, "Checkout");
            outlineBelowItemCart.Controls.Add(doCheckout, 2, 0);
            doCheckout.Click += checkoutButtonClicked;

            addDiscount = CreateButton(AnchorStyles.None | AnchorStyles.Left, "Applicera rabatt");
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

            // Textbox
            itemDescriptionTextbox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Arial", 9)
            };
            outline.Controls.Add(itemDescriptionTextbox, 2, 1);

            discountTextbox = new TextBox()
            {
                Text = "Skriv in rabattkod här...",
                Anchor = AnchorStyles.None,
                Size = new Size(140, 0)
            };
            outline.Controls.Add(discountTextbox, 1, 5);
            discountTextbox.Click += discountTextBoxClicked;
            discountTextbox.KeyDown += discountTextBox_KeyDown;

            // Itemlist (products)
            itemList = new ListBox()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10)
            };
            outline.Controls.Add(itemList);
            outline.SetRowSpan(itemList, 4);
            itemList.SelectedIndexChanged += ItemListBoxClicked;

            // ListView (cart)
            itemCart = new ListView()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                View = View.Details,
                MultiSelect = false,
                Font = new Font("Arial", 10)
            };
            outline.SetRowSpan(itemCart, 4);
            outline.Controls.Add(itemCart, 3, 1);

            itemCart.Columns.Add("Vara").Width = 125;
            itemCart.Columns.Add("Antal").Width = 45;
            itemCart.Columns.Add("Pris").Width = 50;

            // Picturebox
            itemPicture = new PictureBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None,
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            outline.Controls.Add(itemPicture, 1, 1);

            shopItems = Guitar.ReadVendorFile();
        }

        private void loadGuitarToItemListView(object sender, EventArgs e)
        {
            itemList.Items.Clear();
            Guitar.ReadVendorFile();
        }

        private void loadAccessoryToItemListView(object sender, EventArgs e)
        {
            itemList.Items.Clear();
            Accessory.ReadVendorFile();
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
            sumLabel.Text = "";
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
            }
            UpdateListView(shoppingCart);
            UpdateSum(CalculateDictionary());
            loadCart.Enabled = false;
        }

        private void discountTextBoxClicked(object sender, EventArgs e)
        {
            discountTextbox.Text = string.Empty;
        }

        private void discountTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                discountButtonClicked(this, new EventArgs());
            }
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
                discountTextbox.Text = "Du har angivit en rabattkod!";
                discountTextbox.ReadOnly = true;                                // Kanske onödig..
                discountTextbox.Enabled = false;
            }
            else
            {
                discountLabel.Text = "Tyvärr, koden är inte giltig!";
            }
            UpdateSum(CalculateDiscountDictionary());
        }

        private void ItemListBoxClicked(object sender, EventArgs e)
        {
            if (itemList.SelectedIndex < 0)
            {
                return;
            }
            else
            {
                Guitar g = shopItems[itemList.SelectedIndex];
                itemDescriptionTextbox.Text = g.ItemDescr;
                actualPriceLabel.Text = g.ItemPrice.ToString() + " kr";
                itemPicture.Image = Image.FromFile(@"Pictures\Guitars\" + g.ItemPic);
            }
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
                UpdateSum(CalculateDictionary());
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
                UpdateSum(CalculateDictionary());
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

        public static double CalculateDiscountDictionary()
        {
            double sumTotal = 0;
            foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
            {
                sumTotal += (pair.Key.ItemPrice * pair.Value) * (1 - discountGlobalValue / 100);
            }
            return sumTotal;
        }

        private double CalculateDictionary()
        {
            double sumTotal = 0;
            foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
            {
                sumTotal += pair.Key.ItemPrice * pair.Value;
            }
            return sumTotal;
        }

        private void UpdateSum(double sum)
        {
            sumLabel.Text = sum.ToString() + " kr";
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
                Font = new Font("Arial", 12, FontStyle.Bold)
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
            // Icon for receipt
            Icon = new Icon(@"Pictures\guitar_icon.ico");

            // Name of receipt
            Text = "PGCJ Gitarraffär - Tack för att du handlat hos oss.";

            // Accessing global values
            Dictionary<Guitar, int> shoppingCart = MyForm.shoppingCart;
            double discountGlobalValue = MyForm.discountGlobalValue;

            // Sets receipt-size
            ClientSize = new Size(400, 300);

            // Outline
            TableLayoutPanel outlineReceipt = new TableLayoutPanel
            {
                RowCount = 6,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            Controls.Add(outlineReceipt);

            // Rows & Columns
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Absolute, 5));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            outlineReceipt.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < 2; i++) { outlineReceipt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); }

            // Calculation of total price in receipt
            double sum = MyForm.CalculateDiscountDictionary();

            // Labels
            Label receipt = new Label()
            {
                Anchor = AnchorStyles.Left,
                Text = "Kvitto",
                Font = new Font("Arial", 14, FontStyle.Bold | FontStyle.Underline),
            };
            outlineReceipt.Controls.Add(receipt, 0, 0);

            Label totalPriceLabel = new Label()
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Text = sum.ToString() + " kr",
                Font = new Font("Arial", 12, FontStyle.Bold),
            };
            outlineReceipt.Controls.Add(totalPriceLabel, 2, 4);

            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Left, "Artikel"), 0, 2); // itemName
            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Left, "Antal"), 1, 2); // quantity
            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Right | AnchorStyles.Top, "Totalpris:"), 1, 4); // totalPriceLabelText
            outlineReceipt.Controls.Add(CreateLabel(AnchorStyles.Left, "á-Pris"), 2, 2); // pricePrice

            // DataGridView
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

            // DataGridView cellstyle
            dtgv.DefaultCellStyle.SelectionBackColor = dtgv.DefaultCellStyle.BackColor;
            dtgv.DefaultCellStyle.SelectionForeColor = dtgv.DefaultCellStyle.ForeColor;

            // DataGridViewColumns
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

            // Adding columns to DataGridView
            dtgv.Columns.AddRange(new DataGridViewColumn[]
            {
                itemNameGrid, quantityGrid, priceGrid
            });

            // Sums dictionary and displays at receipt
            foreach (KeyValuePair<Guitar, int> pair in shoppingCart)
            {
                dtgv.Rows.Add(pair.Key.ItemName, pair.Value, pair.Key.ItemPrice + " kr");
            }
        }

        private static Label CreateLabel(AnchorStyles anchor, string name)
        {
            return new Label
            {
                Anchor = anchor,
                Text = name,
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
        }
    }
}