using EloBuddy.SDK.Notifications;
using EloBuddy;
using UBAddons.Properties;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Rendering;

namespace UBAddons.Log
{
    public class UBNotification : INotification
    {       
        private readonly string _headerText;
        private readonly string _contentText;
        private readonly Color _headerColor;
        private readonly Color _contentColor = Color.PaleGreen;

        public string FontName
        {
            get
            {
                return "Gill Sans MT Pro Medium";
            }
        }

        public Color HeaderColor
        {
            get
            {
                return _headerColor;
            }
        }
        public Color ContentColor
        {
            get
            {
                return _contentColor;
            }
        }
        public int RightPadding
        {
            get
            {
                return 0;
            }
        }

        public string HeaderText
        {
            get
            {
                return _headerText;
            }
        }

        public string ContentText
        {
            get
            {
                return _contentText;
            }
        }
        public NotificationTexture Texture
        {
            get
            {
                return NotificationTextureTexture;
            }
        }

        private static TextureLoader TextureLoader = new TextureLoader();
        private readonly NotificationTexture NotificationTextureTexture;
        private static readonly NotificationTexture Blue;
        private static readonly NotificationTexture Update;
        private static readonly NotificationTexture Notif;
        private static readonly NotificationTexture Error;
        private static readonly NotificationTexture Warning;
        private static readonly NotificationTexture Recall;

        static UBNotification()
        {
            #region Variables
            NotificationTexture notificationTexture = new NotificationTexture();
            Vector2 pos1 = new Vector2(0f);
            Vector2 pos2 = new Vector2(0f, 1f);
            Vector2 pos3 = new Vector2(0f, 99f);
            Rectangle rec1 = new Rectangle(0, 0, 240, 1);
            Rectangle rec2 = new Rectangle(0, 1, 240, 98);
            Rectangle rec3 = new Rectangle(0, 99, 240, 1);

            NotificationTexture head = notificationTexture;
            NotificationTexture.PartialTexture partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            NotificationTexture content = notificationTexture;
            NotificationTexture.PartialTexture partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            NotificationTexture foot = notificationTexture;
            NotificationTexture.PartialTexture partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            #endregion

            #region LoadBlue  
            notificationTexture = new NotificationTexture();
            head = notificationTexture;
            partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            content = notificationTexture;
            partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            foot = notificationTexture;
            partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            TextureLoader.Load("Blue", Resources.Notification_Blue);
            partialTexture.Texture = (() => TextureLoader["Blue"]);
            head.Header = partialTexture;
            partialTexture2.Texture = (() => TextureLoader["Blue"]);
            content.Content = partialTexture2;
            partialTexture3.Texture = (() => TextureLoader["Blue"]);
            foot.Footer = partialTexture3;
            Blue = notificationTexture;
            #endregion          

            #region LoadUpdated
            notificationTexture = new NotificationTexture();
            head = notificationTexture;
            partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            content = notificationTexture;
            partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            foot = notificationTexture;
            partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            TextureLoader.Load("Update", Resources.Notification_Updated);
            partialTexture.Texture = (() => TextureLoader["Update"]);
            head.Header = partialTexture;
            partialTexture2.Texture = (() => TextureLoader["Update"]);
            content.Content = partialTexture2;
            partialTexture3.Texture = (() => TextureLoader["Update"]);
            foot.Footer = partialTexture3;
            Update = notificationTexture;
            #endregion

            #region LoadWarning
            notificationTexture = new NotificationTexture();
            head = notificationTexture;
            partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            content = notificationTexture;
            partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            foot = notificationTexture;
            partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            TextureLoader.Load("Warning", Resources.Notification_Error);
            partialTexture.Texture = (() => TextureLoader["Warning"]);
            head.Header = partialTexture;
            partialTexture2.Texture = (() => TextureLoader["Warning"]);
            content.Content = partialTexture2;
            partialTexture3.Texture = (() => TextureLoader["Warning"]);
            foot.Footer = partialTexture3;
            Error = notificationTexture;
            #endregion

            #region LoadNotif
            notificationTexture = new NotificationTexture();
            head = notificationTexture;
            partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            content = notificationTexture;
            partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            foot = notificationTexture;
            partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            TextureLoader.Load("Notif", Resources.Notification_Notification);
            partialTexture.Texture = (() => TextureLoader["Notif"]);
            head.Header = partialTexture;
            partialTexture2.Texture = (() => TextureLoader["Notif"]);
            content.Content = partialTexture2;
            partialTexture3.Texture = (() => TextureLoader["Notif"]);
            foot.Footer = partialTexture3;
            Notif = notificationTexture;
            #endregion

            #region LoadYellow  
            notificationTexture = new NotificationTexture();
            head = notificationTexture;
            partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            content = notificationTexture;
            partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            foot = notificationTexture;
            partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            TextureLoader.Load("Yellow", Resources.Notification_Warning);
            partialTexture.Texture = (() => TextureLoader["Yellow"]);
            head.Header = partialTexture;
            partialTexture2.Texture = (() => TextureLoader["Yellow"]);
            content.Content = partialTexture2;
            partialTexture3.Texture = (() => TextureLoader["Yellow"]);
            foot.Footer = partialTexture3;
            Warning = notificationTexture;
            #endregion

            #region LoadPurple
            notificationTexture = new NotificationTexture();
            head = notificationTexture;
            partialTexture = new NotificationTexture.PartialTexture()
            {
                Position = pos1,
                SourceRectangle = rec1
            };
            content = notificationTexture;
            partialTexture2 = new NotificationTexture.PartialTexture()
            {
                Position = pos2,
                SourceRectangle = rec2
            };
            foot = notificationTexture;
            partialTexture3 = new NotificationTexture.PartialTexture()
            {
                Position = pos3,
                SourceRectangle = rec3
            };
            TextureLoader.Load("Recall", Resources.Notification_Recall);
            partialTexture.Texture = (() => TextureLoader["Recall"]);
            head.Header = partialTexture;
            partialTexture2.Texture = (() => TextureLoader["Recall"]);
            content.Content = partialTexture2;
            partialTexture3.Texture = (() => TextureLoader["Recall"]);
            foot.Footer = partialTexture3;
            Recall = notificationTexture;
            #endregion

        }
        public UBNotification(string header, string content, string type)
        {
            _headerText = header;
            _contentText = content;
            if (type.Contains("update"))
            {
                NotificationTextureTexture = Update;
                _headerColor = Color.Lime;
            }
            if (type.Contains("notification"))
            {
                NotificationTextureTexture = Notif;
                _headerColor = Color.DeepPink; //.FromArgb(255, 2, 94);
            }
            if (type.Contains("blue"))
            {
                NotificationTextureTexture = Blue;
                _headerColor = Color.Cyan;
            }
            if (type.Contains("warn"))
            {
                NotificationTextureTexture = Warning;
                _headerColor = Color.Yellow;
            }
            if (type.Contains("recall"))
            {
                NotificationTextureTexture = Recall;
                _headerColor = Color.SlateBlue; //.FromArgb(99, 2, 255);
            }
            if (type.Contains("outdate") || type.Contains("error"))
            {
                NotificationTextureTexture = Error;
                _headerColor = Color.Red;
            }
        }

        private static float LastNotificationTime = 0;
        private static string LastText { get; set; }

        public static void ShowNotif(string Header, string Text, string Type, int Time = 7000, bool ignoretime = false)
        {
            if (ignoretime || !Text.Equals(LastText))
            {
                Notifications.Show(new UBNotification(Header, Text, Type), Time);
                LastText = Text;
            }
            else
            {
                if (LastNotificationTime + Time / 1000 <= Game.Time)
                {
                    Notifications.Show(new UBNotification(Header, Text, Type), Time);
                    LastText = Text;
                    LastNotificationTime = Game.Time;
                }
            }
        }      
    }
}
