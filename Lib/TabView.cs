using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace TabView4
{
    [ContentProperty("TabsContent")]
    public class TabView : Grid
    {
        private Tab selectedTab;
        private Grid headerGrid;
        private List<View> contentList = new List<View>();

        public TabView()
        {
            RowSpacing = 0;
            headerGrid = new Grid();
            headerGrid.ColumnSpacing = 0;
            SetRow(headerGrid, 0);
            Children.Add(headerGrid);


            // Header & Content
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        }

        protected override void OnParentSet()
        {
            InitTabs();
        }

        /// <summary>
        /// Tabs
        /// </summary>
        public static readonly BindableProperty TabsProperty =
            BindableProperty.Create(nameof(Tabs), typeof(IList<Tab>), typeof(TabView),
                defaultValueCreator: b =>
                {
                    return new List<Tab>();
                },
                propertyChanged: (b, o, n) =>
                {
                    (b as TabView).InitTabs();
                });
        public IList<Tab> Tabs => TabsContent;
        public IList<Tab> TabsContent
        {
            get { return (IList<Tab>)GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        /// <summary>
        /// Head background color
        /// </summary>
        public static readonly BindableProperty HeadBackgroundColorProperty =
            BindableProperty.Create(nameof(HeadBackgroundColor), typeof(Color), typeof(TabView), null,
                propertyChanged: (b, o, n) =>
                {
                    var self = (TabView)b;
                    self.headerGrid.BackgroundColor = (Color)n;
                });
        public Color HeadBackgroundColor
        {
            get { return (Color)GetValue(HeadBackgroundColorProperty); }
            set { SetValue(HeadBackgroundColorProperty, value); }
        }

        private void InitTabs()
        {
            // Prepare
            headerGrid.Children.Clear();
            headerGrid.ColumnDefinitions.Clear();
            foreach (var item in contentList)
            {
                Children.Remove(item);
            }
            contentList.Clear();
            selectedTab = null;

            if (Tabs == null)
                return;

            // Start init
            int i = 0;
            int actualId = 0;
            foreach (var item in Tabs)
            {
                item.BindingContext = this;
                item.TabHost = this;
                item.TabId = i;

                var tab = new Grid();
                tab.RowDefinitions.Add(new RowDefinition { Height = 36 });
                tab.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                tab.RowDefinitions.Add(new RowDefinition { Height = 5 });
                tab.Children.Add(item.ImageIcon);
                tab.Children.Add(item.LabelTitle);
                tab.Children.Add(item.Footer);

                // Tap event
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (w, e) => { OpenTab(item.TabId); };
                tab.GestureRecognizers.Add(tapGesture);

                Grid.SetRow(item.ImageIcon, 0);
                Grid.SetRow(item.LabelTitle, 1);
                Grid.SetRow(item.Footer, 2);

                if (item.Content != null)
                {
                    SetRow(item.Content, 1);
                    Children.Add(item.Content);
                    contentList.Add(item.Content);
                }


                if (item.IsVisible)
                {
                    headerGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                    SetColumn(tab, actualId);
                    headerGrid.Children.Add(tab);
                    actualId++;

                    if (selectedTab == null)
                    {
                        selectedTab = item;
                        item.Content.IsVisible = true;
                    }
                }

                item.TabCell = tab;
                i++;
            }
        }

        public void UpdateTabs()
        {
            int i = 0;
            foreach (var item in Tabs)
            {
                if (!item.IsVisible)
                {
                    headerGrid.Children.Remove(item.TabCell);
                    var last = headerGrid.ColumnDefinitions.LastOrDefault();
                    if (last != null)
                        headerGrid.ColumnDefinitions.Remove(last);
                }
                else
                {
                    SetColumn(item.TabCell, i);
                    i++;
                }
            }

            if (selectedTab != null && !selectedTab.IsVisible)
            {
                selectedTab.Content.IsVisible = false;
                selectedTab = null;
                foreach (var item in Tabs)
                {
                    if (item.IsVisible)
                    {
                        selectedTab = item;
                        selectedTab.Content.IsVisible = true;
                        break;
                    }
                }
            }
        }

        public void OpenTab(int index)
        {
            var tab = Tabs[index];

            if (selectedTab != null)
            {
                //if (lastTab == tab)
                //    return;

                selectedTab.Content.IsVisible = false;
            }
            else
            {
                foreach (var item in Tabs)
                {
                    item.Content.IsVisible = false;
                }
            }

            tab.Content.IsVisible = true;
            selectedTab = tab;
        }
    }
}
