using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace TabView4
{
    [ContentProperty("TabsContent")]
    [Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public sealed class TabView : Grid
    {
        private readonly Grid headerGrid;
        private Tab selectedTab;

        public TabView()
        {
            // Header & Content
            RowSpacing = 0;
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            headerGrid = new Grid();
            headerGrid.HorizontalOptions = LayoutOptions.FillAndExpand;
            headerGrid.ColumnSpacing = 0;
            SetRow(headerGrid, 0);
            Children.Add(headerGrid);
        }

        private void InitTabs()
        {
            // Prepare
            //Children.Clear();
            //Children.Add(headerGrid);
            //headerGrid.Children.Clear();
            //headerGrid.ColumnDefinitions.Clear();
            selectedTab = null;

            if (Tabs == null)
                return;

            // Start init
            int i = 0;
            int actualId = 0;
            foreach (var item in Tabs)
            {
                item.SetHost(this);
                item.TabId = i;
                headerGrid.Children.Add(item.TabCell);

                // Add content
                if (item.Content != null)
                {
                    SetRow(item.Content, 1);
                    Children.Add(item.Content);
                }

                if (item.IsVisible)
                {
                    headerGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                    SetColumn(item.TabCell, actualId);
                    actualId++;

                    if (selectedTab == null)
                    {
                        selectedTab = item;
                        item.Content.IsVisible = true;
                    }
                    else
                    {
                        item.Content.IsVisible = false;
                    }
                }

                i++;
            }
        }

        internal void UpdateTabsVisibility(Tab tabChanged, bool isVisible)
        {
            tabChanged.TabCell.IsVisible = isVisible;

            foreach (var item in headerGrid.ColumnDefinitions)
            {
                if (tabChanged)
                item
            }

            //if (!isVisible)
            //{
            //    var last = headerGrid.ColumnDefinitions.LastOrDefault();
            //    last.Width = new GridLength(0);
            //    //headerGrid.ColumnDefinitions.Remove(last);

            //}
            //else
            //{
            //    var match = headerGrid.ColumnDefinitions[tabChanged.TabId];
            //    match.Width = GridLength.Star;
            //    //headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            //}

            int i = 0;
            foreach (var item in Tabs)
            {
                if (!item.IsVisible)
                    continue;

                SetColumn(item.TabCell, i);
                i++;
            }

            if (selectedTab == tabChanged && !isVisible)
            {
                selectedTab.Content.IsVisible = false;
                foreach (var item in Tabs)
                {
                    if (item.IsVisible && selectedTab != item)
                    {
                        selectedTab = item;
                        selectedTab.Content.IsVisible = true;
                        break;
                    }
                }

                if (selectedTab == tabChanged)
                    selectedTab = null;
            }
        }

        internal void OpenTab(int index)
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

        protected override void OnParentSet()
        {
            InitTabs();
            base.OnParentSet();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (Tabs?.Count > 0)
            {
                foreach (var item in Tabs)
                    item.BindingContext = BindingContext;
            }
        }

        /// <summary>
        /// Tabs
        /// </summary>
        public static readonly BindableProperty TabsProperty =
            BindableProperty.Create(nameof(Tabs), typeof(List<Tab>), typeof(TabView),
                propertyChanged: (b, o, n) =>
                {
                    (b as TabView).InitTabs();
                },
                defaultValueCreator: b =>
                {
                    return new List<Tab>();
                });
        public List<Tab> Tabs
        {
            get { return (List<Tab>)GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

		[EditorBrowsable(EditorBrowsableState.Never)]
        public List<Tab> TabsContent
        {
            get { return (List<Tab>)GetValue(TabsProperty); }
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
    }
}
