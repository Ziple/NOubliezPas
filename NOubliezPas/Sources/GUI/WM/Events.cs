using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

namespace kT.GUI
{
	/// <summary>
	/// Base class for all events.
	/// </summary>
	public class Event
	{
		/// <summary>
		/// Enum for the different event types.
		/// </summary>
		public enum EventType
		{
			CustomEvent,
			TimeEvent,
			PlayerAssociatedEvent,
			UpdateEvent,
			DrawEvent,
			ResizeEvent,
			ChildResizeEvent,
			MouseEvent,
			KeyEvent,
			TextEnteredEvent,
			KeyCombinationEvent,
			FocusChangedEvent,
			RenderingOrderChangedEvent,
            ClickEvent,
            HoverEvent,
            HoverEndEvent,
		}

		EventType type;
		bool accepted = true;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="realType">Real type of the event.</param>
		public Event(EventType realType)
		{
			type = realType;
		}

		/// <summary>
		/// If not accepted, the event won't be given to the according OnXX method.
		/// </summary>
		public bool Accepted
		{
			get { return accepted; }
			set { accepted = value; }
		}

		/// <summary>
		/// Get the type of the event.
		/// </summary>
		public EventType Type
		{
			get { return type; }
		}
	}

	/// <summary>
	/// Event containing a time reference.
	/// </summary>
	public class TimeEvent : Event
	{
		Stopwatch timeReference;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="timeReference_">Holded time reference.</param>
		public TimeEvent(Stopwatch timeReference_) :
			base(EventType.TimeEvent)
		{
			timeReference = timeReference_;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="timeReference_">Holded time reference.</param>
		/// <param name="realType">Real type of the event.</param>
		public TimeEvent(Stopwatch timeReference_, EventType realType) :
			base(realType)
		{
			timeReference = timeReference_;
		}

		/// <summary>
		/// Get the time reference.
		/// </summary>
		public Stopwatch Time
		{
			get { return timeReference; }
		}
	}

	/// <summary>
	/// Event raised when begins widgets update.
	/// </summary>
	public class UpdateEvent : TimeEvent
	{
		public UpdateEvent(Stopwatch gameTimeReference) :
			base(gameTimeReference, EventType.UpdateEvent)
		{ }
	}

	/// <summary>
	/// Class for draw events.
	/// </summary>
	public class DrawEvent : Event
	{
		/// <summary>The sprite batch is always ready to use.</summary>
		public Painter Painter;
        public bool DebugDraw;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="painter">Painter to draw the widgets with</param>
		public DrawEvent(Painter painter) :
			base(EventType.DrawEvent)
		{
			Painter = painter;
            DebugDraw = false;
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="painter">Painter to draw the widgets with</param>
        public DrawEvent(Painter painter, bool debug) :
            base(EventType.DrawEvent)
        {
            Painter = painter;
            DebugDraw = debug;
        }
	}

	/// <summary>
	/// Class to hold a resize event.
	/// </summary>
	public class ResizeEvent : Event
	{
		public Vector2f RequestedSize;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ResizeEvent(Vector2f requestedSize) :
			base(EventType.ResizeEvent)
		{
			RequestedSize = requestedSize;
		}
	}

	/// <summary>
	/// Class to hold a child resize event.
	/// </summary>
	public class ChildResizeEvent : Event
	{
		public Widget Child;
        public Vector2f RequestedSize;
        public Vector2f NecessaryParentSize = new Vector2f(0f, 0f);

		/// <summary>
		/// Constructor.
		/// </summary>
        public ChildResizeEvent(Widget child, Vector2f requestedSize) :
			base(EventType.ChildResizeEvent)
		{
			Child = child;
			RequestedSize = requestedSize;
		}
	}

	/// <summary>
	/// Class for events raised when the rendering order of the widget change.
	/// </summary>
	public class RenderingOrderChangedEvent : Event
	{
		public enum RenderingOrderChange
		{
			RenderedEarlier,
			RenderedLater
		}

		public RenderingOrderChange OrderChange;

		public RenderingOrderChangedEvent(RenderingOrderChange orderChange_) :
			base(EventType.RenderingOrderChangedEvent)
		{
			OrderChange = orderChange_;
		}
	}

	/// <summary>
	/// Class for mouse events.
	/// </summary>
	public class MouseEvent : Event
	{
		public enum MouseStateChange
		{
			ButtonPressed,
			ButtonReleased,
			WheelScrolled,
			Moved
		}

		public MouseStateChange StateChange;
		public Mouse.Button Button;
		public int WheelDelta;
        public Vector2f Position;
		public Vector2f PositionDelta;

		public MouseEvent(MouseStateChange type_,
						  Mouse.Button button_,
						  int wheelDelta_,
                          Vector2f position_,
						  Vector2f positionDelta_) :
			base(EventType.MouseEvent)
		{
			StateChange = type_;
			Button = button_;
			WheelDelta = wheelDelta_;
            Position = position_;
			PositionDelta = positionDelta_;
		}
	}

    /// <summary>
    /// Class for click events.
    /// </summary>
    public class ClickEvent : Event
    {
        public Mouse.Button Button;

        public ClickEvent(Mouse.Button button_) :
            base(EventType.ClickEvent)
        {
            Button = button_;
        }
    }

    /// <summary>
    /// Class for hover events.
    /// </summary>
    public class HoverEvent : Event
    {
        public HoverEvent() :
            base(EventType.HoverEvent)
        {}
    }

    /// <summary>
    /// Class for hover events.
    /// </summary>
    public class HoverEndEvent : Event
    {
        public HoverEndEvent() :
            base(EventType.HoverEndEvent)
        { }
    }

	/// <summary>
	/// Class for key pressed/released events.
	/// </summary>
	public class KeyEvent : Event
	{
		public enum KeyStateChange
		{
			Released,
			Pressed
		}

		public KeyStateChange StateChange;
		public Keyboard.Key Key;

		public KeyEvent(KeyStateChange type_, Keyboard.Key key_) :
			base(EventType.KeyEvent)
		{
			StateChange = type_;
			Key = key_;
		}
	}

	/// <summary>
	/// Class for text entered events.
	/// </summary>
	public class TextEnteredEvent : Event
	{
		public string Text;

		public TextEnteredEvent(string text_) :
			base(EventType.TextEnteredEvent)
		{
			Text = text_;
		}
	}

	/// <summary>
	/// Class for key combination events. Triggers the specified action.
	/// </summary>
	public class KeyCombinationEvent : Event
	{
		public bool Ctrl;
		public bool Shift;
        public Keyboard.Key Key;

		public KeyCombinationEvent(bool ctrl_,
								   bool shift_,
                                   Keyboard.Key key_) :
			base(EventType.KeyCombinationEvent)
		{
			Ctrl = ctrl_;
			Shift = shift_;
			Key = key_;
		}
	}

	/// <summary>
	/// Class for focus change events.
	/// Such an event is triggered when a widget lose the focus
	/// or get it.
	/// </summary>
	public class FocusChangedEvent : Event
	{
		public enum FocusStateChange
		{
			GainedFocus,
			LostFocus
		}

		public FocusStateChange StateChange;

		public FocusChangedEvent(FocusStateChange stateChange_) :
			base(EventType.FocusChangedEvent)
		{
			StateChange = stateChange_;
		}
	}
}