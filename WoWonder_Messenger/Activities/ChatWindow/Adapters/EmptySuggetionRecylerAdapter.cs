﻿using System;
using Android.App;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using WoWonder.Helpers.Utils;

namespace WoWonder.Activities.ChatWindow.Adapters
{
    public class EmptySuggetionRecylerAdapter : RecyclerView.Adapter
    {
        public class SuggestionMessages
        {
            public string Message { get; set; }

            public string RealMessage { get; set; }
            public int Id { get; set; }
          
        } 
        public event EventHandler<AdapterClickEvents> OnItemClick;
        private readonly JavaList<SuggestionMessages> SuggetionsMessagesList;

        public EmptySuggetionRecylerAdapter(Activity context)
        {
            try
            {
                var activityContext = context;
                SuggetionsMessagesList = new JavaList<SuggestionMessages>();

                SuggestionMessages a1 = new SuggestionMessages
                {
                    Id = 1,
                    Message = activityContext.GetText(Resource.String.Lbl_SuggestionMessages1),
                    RealMessage = activityContext.GetText(Resource.String.Lbl_SuggestionRealMessages1)
                };

                SuggestionMessages a2 = new SuggestionMessages
                {
                    Id = 2,
                    Message = activityContext.GetText(Resource.String.Lbl_SuggestionMessages2),
                    RealMessage = activityContext.GetText(Resource.String.Lbl_SuggestionRealMessages2)
                };

                SuggestionMessages a3 = new SuggestionMessages
                {
                    Id = 3,
                    Message = activityContext.GetText(Resource.String.Lbl_SuggestionMessages3),
                    RealMessage = activityContext.GetText(Resource.String.Lbl_SuggestionRealMessages3)
                };

                SuggestionMessages a4 = new SuggestionMessages
                {
                    Id = 4,
                    Message = activityContext.GetText(Resource.String.Lbl_SuggestionMessages4),
                    RealMessage = activityContext.GetText(Resource.String.Lbl_SuggestionRealMessages4)
                };

                SuggetionsMessagesList.Add(a1);
                SuggetionsMessagesList.Add(a2);
                SuggetionsMessagesList.Add(a3);
                SuggetionsMessagesList.Add(a4);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here //  First RUN

            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Recyler_Suggetion_Msg_Layout, parent, false);

            var vh = new EmptySuggetionRecylerViewHolder(row, OnClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                // Replace the contents of the view with that element
                if (viewHolder is EmptySuggetionRecylerViewHolder holder)
                {
                    var item = SuggetionsMessagesList[position];
                    if (item != null)
                    {
                        holder.NormaText.Text = item.Message;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void OnClick(AdapterClickEvents args)
        {
            OnItemClick?.Invoke(this, args);
        }

        public override int ItemCount => SuggetionsMessagesList?.Count ?? 0;

        public SuggestionMessages GetItem(int position)
        {
            return SuggetionsMessagesList[position];
        }


        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                return 0;
            }
        }

    }

    public class EmptySuggetionRecylerViewHolder : RecyclerView.ViewHolder
        {

            public View MainView { get; private set; }
            public TextView NormaText { get; private set; }

            public EmptySuggetionRecylerViewHolder(View itemView, Action<AdapterClickEvents> listener) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    NormaText = itemView.FindViewById<TextView>(Resource.Id.normalText);
                    
                    itemView.Click += (sender, e) => listener(new AdapterClickEvents
                    {
                        View = itemView,
                        Position = AdapterPosition
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Error Allen"); 
                }
            }
        }

        public class AdapterClickEvents : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

       
   
}