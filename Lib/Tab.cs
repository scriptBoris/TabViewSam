using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace TabViewSam
{
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    [ContentProperty("Content")]
    public sealed class Tab : BindableObject
    {
        internal Grid TabCell;
        internal Label LabelTitle;
        internal Image ImageIcon;
        internal BoxView Footer;
        internal TabView TabHost;
        internal int TabId;
        internal bool IsSelected;

        public Tab()
        {
            TabCell = new Grid();
            TabCell.RowDefinitions.Add(new RowDefinition { Height = 36 });
            TabCell.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            TabCell.RowDefinitions.Add(new RowDefinition { Height = 5 });

            LabelTitle = new Label();
            LabelTitle.HorizontalTextAlignment = TextAlignment.Center;
            LabelTitle.VerticalTextAlignment = TextAlignment.Center;
            Grid.SetRow(LabelTitle, 1);

            ImageIcon = new Image();
            Grid.SetRow(ImageIcon, 0);

            Footer = new BoxView();
            Grid.SetRow(Footer, 2);

            TabCell.Children.Add(ImageIcon);
            TabCell.Children.Add(LabelTitle);
            TabCell.Children.Add(Footer);
        }

        internal void SetHost(TabView host)
        {
            TabHost = host;

            // Tap event
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (o, e) => { host.OpenTab(TabId); };
            TabCell.GestureRecognizers.Add(tapGesture);

            LabelTitle.FontSize = TitleFontSize ?? host.TitleFontSize;
            LabelTitle.TextColor = TitleColor.IsDefault ? TitleColor : host.TitleColor;
            TabCell.BackgroundColor = BackgroundColor.IsDefault ? host.TabsBackgroundColor : BackgroundColor;
        }

        // Title
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(Tab), 
            propertyChanged: (b, o, n) =>
            {
                (b as Tab).LabelTitle.Text = (string)n;
            });
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Title font size
        public static readonly BindableProperty TitleFontSizeProperty =
            BindableProperty.Create(nameof(TitleFontSize), typeof(double?), typeof(Tab), null,
            propertyChanged: (b, o, n) =>
            {
                var self = b as Tab;
                double? value = (double?)n;
                if (value.HasValue)
                    self.LabelTitle.FontSize = value.Value;
                else
                    self.LabelTitle.FontSize = self.TabHost?.TitleFontSize ?? 16.0;
            });
        public double? TitleFontSize
        {
            get { return (double?)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Title color
        public static readonly BindableProperty TitleColorProperty =
            BindableProperty.Create(nameof(TitleColor), typeof(Color), typeof(Tab),
            propertyChanged: (b, o, n) =>
            {
                (b as Tab).LabelTitle.TextColor = (Color)n;
            });
        public Color TitleColor
        {
            get { return (Color)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }

        // Selected color
        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(Tab), null,
                propertyChanged:(b,o,n)=>
                {
                    var self = b as Tab;
                    if (self.IsSelected)
                        self.TabCell.BackgroundColor = (Color)n;
                });
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Selected footer color
        public static readonly BindableProperty SelectedFooterColorProperty =
            BindableProperty.Create(nameof(SelectedFooterColor), typeof(Color), typeof(Tab), null,
                propertyChanged: (b, o, n) =>
                {
                    var self = b as Tab;
                    if (self.IsSelected)
                        self.Footer.BackgroundColor = (Color)n;
                });
        public Color SelectedFooterColor
        {
            get { return (Color)GetValue(SelectedFooterColorProperty); }
            set { SetValue(SelectedFooterColorProperty, value); }
        }

        // Background color
        public static readonly BindableProperty BackgroundColorProperty =
            BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(Tab), null,
                propertyChanged: (b,o,n ) =>
                {
                    var self = b as Tab;
                    if (!self.IsSelected)
                        self.TabCell.BackgroundColor = (Color)n;
                });
        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        // Icon
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(Tab), null,
                propertyChanged: ((b, o, n) =>
                {
                    string value = (string)n;
                    (b as Tab).ImageIcon.Source = value;
                }));
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Content
        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(View), typeof(Tab), null,
                propertyChanged: (b, o, n) =>
                {
                    var self = b as Tab;
                    var content = n as View;
                    if (self.TabHost?.SelectedTab != self)
                        content.IsVisible = false;
                });
        public View Content
        {
            get { return (View)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public View ContentXaml => Content;

        // Is visible
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(Tab), true,
                propertyChanged: (b, o, n) =>
                {
                    var self = (Tab)b;
                    if (self.TabHost != null)
                        self.TabHost.UpdateTabsVisibility(self, (bool)n);
                });
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
    }
}
