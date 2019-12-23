using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace TabView4
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
                    var content = n as View;
                    content.IsVisible = false;
                });
        public View Content
        {
            get { return (View)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public View ContentXaml => Content;
        //{
        //    get { return (View)GetValue(ContentProperty); }
        //    set { SetValue(ContentProperty, value); }
        //}

        //public View Content => ContentSystem;

        // Is visible
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(Tab),
                defaultValueCreator: (b) =>
                {
                    return true;
                },
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

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}
