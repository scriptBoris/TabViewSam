using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TabView4
{
    [ContentProperty("Content")]
    public class Tab : BindableObject
    {
        public Grid TabCell;
        public Label LabelTitle { get; private set; }
        public Image ImageIcon { get; private set; }
        public BoxView Footer { get; private set; }

        public TabView TabHost;
        public int TabId;

        public Tab()
        {
            LabelTitle = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                BindingContext = this,
            };

            ImageIcon = new Image();
            ImageIcon.BindingContext = this;

            Footer = new BoxView();
            Footer.BindingContext = this;
        }

        // Title
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(Tab), string.Empty,
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
            BindableProperty.Create(nameof(ContentSystem), typeof(View), typeof(Tab), null,
                propertyChanged: (b, o, n) =>
                {
                    var content = n as View;
                    content.IsVisible = false;
                });
        public View ContentSystem
        {
            get { return (View)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public View Content => ContentSystem;

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
                        self.TabHost.UpdateTabs();
                });
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
    }
}
