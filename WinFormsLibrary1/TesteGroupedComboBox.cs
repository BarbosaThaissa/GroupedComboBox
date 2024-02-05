using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BufferedPainting;

namespace GroupedComboBox2
{
    public class TesteGroupedComboBox : ComboBox, IComparer
    {
        private BindingSource mBindingSource;
        private PropertyDescriptor mGroupProperty;

        //Eu Coloquei
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


        public string GroupMember
        {
            get { return _groupMember; }
            set
            {
                _groupMember = value;
                if (_bindingSource != null) SyncInternalItems();
            }
        }

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
                    base.DataSource = _bindingSource = null;
                }
            }
        }

        private void mBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            SyncInternalItems();
        }

        public TesteGroupedComboBox()
        {
            base.DrawMode = DrawMode.OwnerDrawVariable;
            _groupMember = String.Empty;
            _internalItems = new ArrayList();
            _textFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;

            _bufferedPainter = new BufferedPainter<ComboBoxState>(this);
            _bufferedPainter.DefaultState = ComboBoxState.Normal;
            //_bufferedPainter.PaintVisualState += new EventHandler<BufferedPaintEventArgs<ComboBoxState>>(_bufferedPainter_PaintVisualState);
            _bufferedPainter.AddTransition(ComboBoxState.Normal, ComboBoxState.Hot, 250);
            _bufferedPainter.AddTransition(ComboBoxState.Hot, ComboBoxState.Normal, 350);
            _bufferedPainter.AddTransition(ComboBoxState.Pressed, ComboBoxState.Normal, 350);
            _groupFontBold = new Font(Font, FontStyle.Bold);

            //ToggleStyle();
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
                base.DataSource = _internalSource;
            }
            else
            {
                _internalSource.ResetBindings(false);
            }
        }




        int IComparer.Compare(object x, object y)
        {
            // compare the display values (and return the result if there is no grouping)
            int secondLevelSort = Comparer.Default.Compare(GetItemText(x), GetItemText(y));
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


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if ((e.Index >= 0) && (e.Index < Items.Count))
            {
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
                    GetItemText(Items[e.Index]),
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
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            e.ItemHeight = Font.Height;

            string groupText;
            if (IsGroupStart(e.Index, out groupText))
            {
                // the first item in each group will be twice as tall in order to accommodate the group header
                e.ItemHeight *= 2;
                e.ItemWidth = Math.Max(
                    e.ItemWidth,
                    TextRenderer.MeasureText(
                        e.Graphics,
                        groupText,
                        _groupFont,
                        new Size(e.ItemWidth, e.ItemHeight),
                        _textFormatFlags
                    ).Width
                );
            }
        }
    }
}

