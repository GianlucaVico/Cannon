using System;
using Gtk;
namespace Cannon_GUI
{
    public class Tile : Gtk.Widget
    {
        protected Gtk.EventBox box;
        protected Gtk.Image image, foreground;
        protected Gtk.Fixed frame;
        public Position Position { get; }

        protected TileManager manager;

        public bool Selected { get; set; }
        public bool Target { get; set; }
        public TileColor Color { get; set; }
        public TileType Type { get; set; }

        // Some constants for the images and some predefines states
        /*public static readonly Gdk.Pixbuf LIGHT_PIECE_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.light_piece.svg");
        public static readonly Gdk.Pixbuf DARK_PIECE_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.dark_piece.svg");
        public static readonly Gdk.Pixbuf EMPTY_PIECE_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.empty.svg");

        public static readonly Gdk.Pixbuf SELECTED_EMPTY_PIECE_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.selected_empty.svg");
        public static readonly Gdk.Pixbuf TARGET_EMPTY_PIECE_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.target_empty.svg");

        public static readonly Gdk.Pixbuf DARK_TOWN_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.dark_town.svg");
        public static readonly Gdk.Pixbuf LIGHT_TOWN_IMG = Gdk.Pixbuf.LoadFromResource("Cannon_GUI.light_town.svg");
        */
        public static readonly Gdk.Pixbuf LIGHT_PIECE_IMG = new Gdk.Pixbuf($"images{Constants.Slash}light_piece.{Constants.ImgExt}");
        public static readonly Gdk.Pixbuf DARK_PIECE_IMG = new Gdk.Pixbuf($"images{Constants.Slash}dark_piece.{Constants.ImgExt}");
        public static readonly Gdk.Pixbuf EMPTY_PIECE_IMG = new Gdk.Pixbuf($"images{Constants.Slash}empty.{Constants.ImgExt}");

        public static readonly Gdk.Pixbuf SELECTED_EMPTY_PIECE_IMG = new Gdk.Pixbuf($"images{Constants.Slash}selected_empty.{Constants.ImgExt}");
        public static readonly Gdk.Pixbuf TARGET_EMPTY_PIECE_IMG = new Gdk.Pixbuf($"images{Constants.Slash}target_empty.{Constants.ImgExt}");

        public static readonly Gdk.Pixbuf DARK_TOWN_IMG = new Gdk.Pixbuf($"images{Constants.Slash}dark_town.{Constants.ImgExt}");
        public static readonly Gdk.Pixbuf LIGHT_TOWN_IMG = new Gdk.Pixbuf($"images{Constants.Slash}light_town.{Constants.ImgExt}");

        public Tile(int x, int y, Gtk.Fixed parent, TileManager manager)
        {
            Position = new Position(x, y);
            this.manager = manager;

            //Copied from the automatic generated code
            box = new EventBox
            {
                Name = "eventbox{x}{y}",
                AboveChild = true,
                VisibleWindow = false
            };

            frame = new Fixed
            {
                Name = "fixed{x}{y}",
                HasWindow = false
            };
            box.Add(frame);


            image = new Image
            {
                Name = $"image{x}{y}",
                Pixbuf = GetImage()
            };
            frame.Add(image);

            foreground = new Image
            {
                Name = $"foreground_image{x}{y}",
                Pixbuf = GetForegroundImage()
            };
            frame.Add(foreground);

            parent.Add(box);
            Fixed.FixedChild obj = ((Fixed.FixedChild)(parent[box]));
            //

            obj.X = Constants.BoardOffSet + x * Constants.TileSize;
            obj.Y = Constants.BoardOffSet + (Constants.Size - 1 - y) * Constants.TileSize;

            box.ButtonPressEvent += OnClick;

            ToEmpy();
        }

        public new void Show()
        {
            image.Show();
            box.Show();
            frame.Show();
            foreground.Show();
        }

        protected void OnClick(object sender, EventArgs e)
        {
            Console.WriteLine(ToString());
            manager.OnClick(this);
        }

        public Gdk.Pixbuf GetImage()
        {
            Gdk.Pixbuf img = EMPTY_PIECE_IMG;
            switch (Type)
            {
                case TileType.Empty:
                    img = EMPTY_PIECE_IMG;
                    break;
                case TileType.Piece:
                    img = (Color == TileColor.Dark) ? DARK_PIECE_IMG : LIGHT_PIECE_IMG;
                    break;
                case TileType.Town:
                    img = (Color == TileColor.Dark) ? DARK_TOWN_IMG : LIGHT_TOWN_IMG;
                    break;
            }
            return img;
        }

        public Gdk.Pixbuf GetForegroundImage()
        {
            Gdk.Pixbuf img = EMPTY_PIECE_IMG;
            if (Selected)
            {
                img = SELECTED_EMPTY_PIECE_IMG;
            }
            else if (Target)
            {
                img = TARGET_EMPTY_PIECE_IMG;
            }
            return img;
        }

        public void Update()
        {
            this.image.Pixbuf = GetImage();
            this.foreground.Pixbuf = GetForegroundImage();
        }

        #region ChangeLook
        public Tile ToDarkPiece()
        {
            Selected = false;
            Target = false;
            Color = TileColor.Dark;
            Type = TileType.Piece;
            return this;
        }

        public Tile ToLightPiece()
        {
            Selected = false;
            Target = false;
            Color = TileColor.Light;
            Type = TileType.Piece;
            return this;
        }

        public Tile ToDarkTown()
        {
            Selected = false;
            Target = false;
            Color = TileColor.Dark;
            Type = TileType.Town;
            return this;
        }

        public Tile ToLightTown()
        {
            Selected = false;
            Target = false;
            Color = TileColor.Light;
            Type = TileType.Town;
            return this;
        }

        public Tile ToEmpy()
        {
            Selected = false;
            Target = false;
            Color = TileColor.No;
            Type = TileType.Empty;
            return this;
        }

        public void ToggleSelect()
        {
            Selected = !Selected;
        }
        #endregion

        public override string ToString() => $"{this.Position} - Selected {Selected} - Target {Target} - Color {Color} - Type {Type}";
    }
}
