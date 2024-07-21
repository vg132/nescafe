using Nescafe.Core;

namespace Nescafe.UI.Input;

public interface IInput
{
}

public class Keyboard
{
	private readonly Form _form;
	private readonly Core.Console _console;

	public Keyboard(Form form, Core.Console console)
	{
		_form = form;
		_console = console;

		_form.KeyDown += OnKeyDown;
		_form.KeyUp += OnKeyUp;
	}

	private void OnKeyDown(object sender, KeyEventArgs e)
	{
		SetControllerButton(true, e);
	}

	private void OnKeyUp(object sender, KeyEventArgs e)
	{
		SetControllerButton(false, e);
	}

	public void SetControllerButton(bool state, KeyEventArgs e)
	{
		if (_console?.IsRunning == true)
		{
			switch (e.KeyCode)
			{
				case Keys.Z:
					_console.Controller.setButtonState(Controller.Button.A, state);
					break;
				case Keys.X:
					_console.Controller.setButtonState(Controller.Button.B, state);
					break;
				case Keys.Left:
					_console.Controller.setButtonState(Controller.Button.Left, state);
					break;
				case Keys.Right:
					_console.Controller.setButtonState(Controller.Button.Right, state);
					break;
				case Keys.Up:
					_console.Controller.setButtonState(Controller.Button.Up, state);
					break;
				case Keys.Down:
					_console.Controller.setButtonState(Controller.Button.Down, state);
					break;
				case Keys.Q:
				case Keys.Enter:
					_console.Controller.setButtonState(Controller.Button.Start, state);
					break;
				case Keys.W:
				case Keys.Escape:
					_console.Controller.setButtonState(Controller.Button.Select, state);
					break;
			}
		}
	}
}