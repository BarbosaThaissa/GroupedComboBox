using BufferedPainting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CustomComboBox.Controls
{ /*
     * Classe extraída de: https://rjcodeadvance.com/custom-combobox-icon-back-text-border-color-winform-c/
     */

    [DefaultEvent("OnSelectedIndexChanged")]
    public partial class CustomGroupedComboBox : UserControl, IComparer
    {
        //Fields
        private Color backColor = Color.WhiteSmoke;
        private Color iconColor = Color.MediumSlateBlue;
        private Color listBackColor = Color.FromArgb(230, 228, 245);
        private Color listTextColor = Color.DimGray;
        private Color borderColor = Color.MediumSlateBlue;
        private int borderSize = 1;

        //Items
        private ComboBox cmbList;
        private Label lblText;
        private Button btnIcon;

        //EU QUE COLOQUEI
        private BindingSource _bindingSource;                       // used for change detection and grouping
        private Font _groupFont;                                    // for painting
        private string _groupMember;                                // name of group-by property
        private PropertyDescriptor _groupProperty;                  // used to get group-by values
        private ArrayList _internalItems;                           // internal sorted collection of items
        private BindingSource _internalSource;                      // binds sorted collection to the combobox
        private TextFormatFlags _textFormatFlags;                   // used in measuring/painting
        private BufferedPainter<ComboBoxState> _bufferedPainter;    // provides buffered paint animations
        private bool _isNotDroppedDown;
        private Font _groupFontBold;


        //Events
        public event EventHandler OnSelectedIndexChanged; //Default event
        public event EventHandler OnTextChanged;
        public CustomGroupedComboBox()
        {
            cmbList = new ComboBox();
            lblText = new Label();
            btnIcon = new Button();
            this.SuspendLayout();

            //ComboBox: Dropdown list
            cmbList.BackColor = listBackColor;
            cmbList.Font = new Font(this.Font.Name, 10F);
            cmbList.ForeColor = listTextColor;
            cmbList.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);//Default event
            cmbList.TextChanged += new EventHandler(ComboBox_TextChanged);//Refresh text
            cmbList.FormattingEnabled = true;
            //Button: Icon
            btnIcon.Dock = DockStyle.Right;
            btnIcon.FlatStyle = FlatStyle.Flat;
            btnIcon.FlatAppearance.BorderSize = 0;
            btnIcon.BackColor = backColor;
            btnIcon.Size = new Size(30, 30);
            btnIcon.Cursor = Cursors.Hand;
            btnIcon.Click += new EventHandler(Icon_Click);//Open dropdown list
            btnIcon.Paint += new PaintEventHandler(Icon_Paint);//Draw icon

            //Label: Text
            lblText.Dock = DockStyle.Fill;
            lblText.AutoSize = false;
            lblText.BackColor = backColor;
            lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblText.Padding = new Padding(8, 0, 0, 0);
            lblText.Font = new Font(this.Font.Name, 10F);
            //->Attach label events to user control event
            lblText.Click += new EventHandler(Surface_Click);//Select combo box
            lblText.MouseEnter += new EventHandler(Surface_MouseEnter);
            lblText.MouseLeave += new EventHandler(Surface_MouseLeave);
            //EU QUE COLOQUEI
            cmbList.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
            cmbList.MeasureItem += new MeasureItemEventHandler(ComboBox_MeasureItem);
            _groupFontBold = new Font(Font, FontStyle.Bold);
            cmbList.DrawMode = DrawMode.OwnerDrawVariable;
            _groupMember = String.Empty;
            _internalItems = new ArrayList();
            _textFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;

            //_bufferedPainter = new BufferedPainter<ComboBoxState>(this);
            //_bufferedPainter.DefaultState = ComboBoxState.Normal;
            //_bufferedPainter.PaintVisualState += new EventHandler<BufferedPaintEventArgs<ComboBoxState>>(_bufferedPainter_PaintVisualState);
            //_bufferedPainter.AddTransition(ComboBoxState.Normal, ComboBoxState.Hot, 250);
            //_bufferedPainter.AddTransition(ComboBoxState.Hot, ComboBoxState.Normal, 350);
            //_bufferedPainter.AddTransition(ComboBoxState.Pressed, ComboBoxState.Normal, 350);

            //User Control
            this.Controls.Add(lblText);//2
            this.Controls.Add(btnIcon);//1
            this.Controls.Add(cmbList);//0
            this.MinimumSize = new Size(200, 30);
            this.Size = new Size(200, 30);
            this.ForeColor = Color.DimGray;
            this.Padding = new Padding(borderSize);//Border Size
            this.Font = new Font(this.Font.Name, 10F);
            base.BackColor = borderColor; //Border Color
            this.ResumeLayout();
            AdjustComboBoxDimensions();
        }

        //Private methods
        private void AdjustComboBoxDimensions()
        {
            cmbList.Width = lblText.Width;
            cmbList.Location = new Point()
            {
                X = this.Width - this.Padding.Right - cmbList.Width,
                Y = lblText.Bottom - cmbList.Height
            };
        }

        //Event methods

        //-> Default event
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged.Invoke(sender, e);
            //Refresh text
            lblText.Text = cmbList.Text;
        }
        //-> Items actions
        private void Icon_Click(object sender, EventArgs e)
        {
            //Open dropdown list
            cmbList.Select();
            cmbList.DroppedDown = true;
        }
        private void Surface_Click(object sender, EventArgs e)
        {
            //Attach label click to user control click
            this.OnClick(e);
            //Select combo box
            cmbList.Select();
            if (cmbList.DropDownStyle == ComboBoxStyle.DropDownList)
                cmbList.DroppedDown = true;//Open dropdown list
        }
        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            //Refresh text
            lblText.Text = cmbList.Text;

            OnTextChanged?.Invoke(sender, e);
        }

        //-> Draw icon
        private void Icon_Paint(object sender, PaintEventArgs e)
        {
            //Fields
            int iconWidht = 14;
            int iconHeight = 6;
            var rectIcon = new Rectangle((btnIcon.Width - iconWidht) / 2, (btnIcon.Height - iconHeight) / 2, iconWidht, iconHeight);
            Graphics graph = e.Graphics;

            //Draw arrow down icon
            using (GraphicsPath path = new GraphicsPath())
            using (Pen pen = new Pen(iconColor, 2))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                path.AddLine(rectIcon.X, rectIcon.Y, rectIcon.X + (iconWidht / 2), rectIcon.Bottom);
                path.AddLine(rectIcon.X + (iconWidht / 2), rectIcon.Bottom, rectIcon.Right, rectIcon.Y);
                graph.DrawPath(pen, path);
            }
        }

        public int FindString(string nome)
        {
            return cmbList.FindString(nome);
        }

        //Properties
        //-> Aparência
        [Category("Customização - Aparência")]
        public new Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                lblText.BackColor = backColor;
                btnIcon.BackColor = backColor;
            }
        }

        [Category("Customização - Aparência")]
        public Color IconColor
        {
            get { return iconColor; }
            set
            {
                iconColor = value;
                btnIcon.Invalidate();//Redraw icon
            }
        }

        [Category("Customização - Aparência")]
        public Color ListBackColor
        {
            get { return listBackColor; }
            set
            {
                listBackColor = value;
                cmbList.BackColor = listBackColor;
            }
        }

        [Category("Customização - Aparência")]
        public Color ListTextColor
        {
            get { return listTextColor; }
            set
            {
                listTextColor = value;
                cmbList.ForeColor = listTextColor;
            }
        }

        [Category("Customização - Aparência")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                base.BackColor = borderColor; //Border Color
            }
        }

        [Category("Customização - Aparência")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Padding = new Padding(borderSize);//Border Size
                AdjustComboBoxDimensions();
            }
        }

        [Category("Customização - Aparência")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                lblText.ForeColor = value;
            }
        }

        [Category("Customização - Aparência")]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                lblText.Font = value;
                cmbList.Font = value;//Optional
            }
        }

        [Category("Customização - Aparência")]
        public string Texts
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        [Category("Customização - Aparência")]
        public ComboBoxStyle DropDownStyle
        {
            get { return cmbList.DropDownStyle; }
            set
            {
                if (cmbList.DropDownStyle != ComboBoxStyle.Simple)
                    cmbList.DropDownStyle = value;
            }
        }

        //Properties
        //-> Data
        [Category("Customização - Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ComboBox.ObjectCollection Items
        {
            get { return cmbList.Items; }
        }

        [Category("Customização - Data")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        /* public object DataSource
        {
            get { return cmbList.DataSource; }
            set { cmbList.DataSource = value; }
        } */
        public new object DataSource
        {
            get
            {
                // binding source should be transparent to the user
                return (_bindingSource != null) ? _bindingSource.DataSource : null;
            }
            set
            {
                _internalSource = null;

                if (value != null)
                {
                    // wrap the object in a binding source and listen for changes
                    _bindingSource = new BindingSource(value, String.Empty);
                    _bindingSource.ListChanged += new ListChangedEventHandler(mBindingSource_ListChanged);
                    SyncInternalItems();
                }
                else
                {
                    // remove binding
                    cmbList.DataSource = _bindingSource = null;
                }
            }
        }

        private void mBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            SyncInternalItems();
        }

        [Category("Customização - Data")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get { return cmbList.AutoCompleteCustomSource; }
            set { cmbList.AutoCompleteCustomSource = value; }
        }

        [Category("Customização - Data")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteSource.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get { return cmbList.AutoCompleteSource; }
            set { cmbList.AutoCompleteSource = value; }
        }

        [Category("Customização - Data")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteMode.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get { return cmbList.AutoCompleteMode; }
            set { cmbList.AutoCompleteMode = value; }
        }

        [Category("Customização - Data")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get { return cmbList.SelectedItem; }
            set { cmbList.SelectedItem = value; }
        }

        [Category("Customização - Data")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedValue
        {
            get { return cmbList.SelectedValue; }
            set { cmbList.SelectedValue = value; }
        }

        [Category("Customização - Data")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get { return cmbList.SelectedIndex; }
            set { cmbList.SelectedIndex = value; }
        }

        [Category("Customização - Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayMember
        {
            get { return cmbList.DisplayMember; }
            set { cmbList.DisplayMember = value; }
        }

        [Category("Customização - Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get { return cmbList.ValueMember; }
            set { cmbList.ValueMember = value; }
        }

        //EU QUE COLOQUEI
        public string GroupMember
        {
            get { return _groupMember; }
            set
            {
                _groupMember = value;
                if (_bindingSource != null) SyncInternalItems();
            }
        }
        int IComparer.Compare(object x, object y)
        {
            // compare the display values (and return the result if there is no grouping)
            int secondLevelSort = Comparer.Default.Compare(cmbList.GetItemText(x), cmbList.GetItemText(y));
            if (_groupProperty == null) return secondLevelSort;

            // compare the group values - if equal, return the earlier comparison
            int firstLevelSort = Comparer.Default.Compare(
                Convert.ToString(_groupProperty.GetValue(x)),
                Convert.ToString(_groupProperty.GetValue(y))
            );

            if (firstLevelSort == 0)
                return secondLevelSort;
            else
                return firstLevelSort;
        }


        private bool IsGroupStart(int index, out string groupText)
        {
            bool isGroupStart = false;
            groupText = String.Empty;

            if ((_groupProperty != null) && (index >= 0) && (index < Items.Count))
            {
                // get the group value using the property descriptor
                groupText = Convert.ToString(_groupProperty.GetValue(Items[index]));

                // this item is the start of a group if it is the first item with a group -or- if
                // the previous item has a different group
                if ((index == 0) && (groupText != String.Empty))
                {
                    isGroupStart = true;
                }
                else if ((index - 1) >= 0)
                {
                    string previousGroupText = Convert.ToString(_groupProperty.GetValue(Items[index - 1]));
                    if (previousGroupText != groupText) isGroupStart = true;
                }
            }

            return isGroupStart;
        }

        private void SyncInternalItems()
        {
            // locate the property descriptor that corresponds to the value of GroupMember
            _groupProperty = null;
            foreach (PropertyDescriptor descriptor in _bindingSource.GetItemProperties(null))
            {
                if (descriptor.Name.Equals(_groupMember))
                {
                    _groupProperty = descriptor;
                    break;
                }
            }

            // rebuild the collection and sort using custom logic
            _internalItems.Clear();
            foreach (object item in _bindingSource) _internalItems.Add(item);
            _internalItems.Sort(this);

            // bind the underlying ComboBox to the sorted collection
            if (_internalSource == null)
            {
                _internalSource = new BindingSource(_internalItems, String.Empty);
                cmbList.DataSource = _internalSource;
            }
            else
            {
                _internalSource.ResetBindings(false);
            }
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Se o índice estiver fora do intervalo, não faça nada
            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            // get noteworthy states
            bool comboBoxEdit = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool noAccelerator = (e.State & DrawItemState.NoAccelerator) == DrawItemState.NoAccelerator;
            bool disabled = (e.State & DrawItemState.Disabled) == DrawItemState.Disabled;
            bool focus = (e.State & DrawItemState.Focus) == DrawItemState.Focus;

            // determine grouping
            string groupText;
            bool isGroupStart = IsGroupStart(e.Index, out groupText) && !comboBoxEdit;
            bool hasGroup = (groupText != String.Empty) && !comboBoxEdit;

            // the item text will appear in a different colour, depending on its state
            Color textColor;
            if (disabled)
                textColor = SystemColors.GrayText;
            else if (!comboBoxEdit && selected)
                textColor = SystemColors.HighlightText;
            else
                textColor = ForeColor;

            // items will be indented if they belong to a group
            Rectangle itemBounds = Rectangle.FromLTRB(
                e.Bounds.X + (hasGroup ? 12 : 0),
                e.Bounds.Y + (isGroupStart ? (e.Bounds.Height / 2) : 0),
                e.Bounds.Right,
                e.Bounds.Bottom
            );
            Rectangle groupBounds = new Rectangle(
                e.Bounds.X,
                e.Bounds.Y,
                e.Bounds.Width,
                e.Bounds.Height / 2
            );

            if (isGroupStart && selected)
            {
                // ensure that the group header is never highlighted
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                e.Graphics.FillRectangle(new SolidBrush(BackColor), groupBounds);
            }
            else if (disabled)
            {
                // disabled appearance
                e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.Bounds);
            }
            else if (!comboBoxEdit)
            {
                // use the default background-painting logic
                e.DrawBackground();
            }

            // render group header text
            if (isGroupStart)
            {
                // Use a fonte em negrito para o texto do grupo
                TextRenderer.DrawText(
                    e.Graphics,
                    groupText,
                    _groupFontBold, // Aqui estamos usando a fonte em negrito
                    groupBounds,
                    ForeColor,
                    _textFormatFlags
                );
            }

            // render item text
            TextRenderer.DrawText(
                e.Graphics,
                cmbList.GetItemText(Items[e.Index]),
                Font,
                itemBounds,
                textColor,
                _textFormatFlags
            );

            // paint the focus rectangle if required
            if (focus && !noAccelerator)
            {
                if (isGroupStart && selected)
                {
                    // don't draw the focus rectangle around the group header
                    ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.FromLTRB(groupBounds.X, itemBounds.Y, itemBounds.Right, itemBounds.Bottom));
                }
                else
                {
                    // use default focus rectangle painting logic
                    e.DrawFocusRectangle();
                }
            }
        }
        private void ComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            // Se o índice estiver fora do intervalo, não faça nada
            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            e.ItemHeight = Font.Height;

            // determine grouping
            string groupText;
            if (IsGroupStart(e.Index, out groupText))
            {
                // the first item in each group will be twice as tall in order to accommodate the group header
                e.ItemHeight *= 2;

                // measure the width based on the group header text
                e.ItemWidth = Math.Max(
                    e.ItemWidth,
                    TextRenderer.MeasureText(
                        groupText,
                        _groupFont,
                        new Size(e.ItemWidth, e.ItemHeight),
                        _textFormatFlags
                    ).Width
                );
            }
        }

        //EU QUE COLOQUEI

        //->Attach label events to user control event
        private void Surface_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        private void Surface_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        //Overridden methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustComboBoxDimensions();
        }
    }

}
