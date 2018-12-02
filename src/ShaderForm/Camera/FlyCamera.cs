using OpenTK;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShaderForm.Camera
{
	public class FlyCamera
	{
		public FlyCamera()
		{
			Reset();
		}

		public Vector3 Position;
		public Vector3 Rotation;
		public float Speed;

		public bool IsActive => time.IsRunning;

		public void Reset()
		{
			Position = new Vector3(0);
			Rotation = new Vector3(0);
			Speed = 1f;
			time.Reset();
		}

		public void Update(float mouseX, float mouseY, bool leftPressed)
		{
			Vector3 rot = MathHelper.DegreesToRadians(1f) * Rotation;
			float mouseXDelta = mouseX - lastMouseX;
			lastMouseX = mouseX;
			float mouseYDelta = mouseY - lastMouseY;
			lastMouseY = mouseY;

			// x-Rotation
			if (leftPressed) rot.X += 0.001f * mouseYDelta;
			rot.X = (float)(rot.X % (2.0 * Math.PI));

			// y-Rotation
			if (leftPressed) rot.Y += 0.0005f * mouseXDelta;
			rot.Y = (float)(rot.Y % (2.0 * Math.PI));

			var total = (float)time.Elapsed.TotalSeconds;
			var dt = total - lastTime;
			lastTime = total;

			// rotate by keys
			if(IsActive)
			{
				rot.X += tilt * Speed * dt;
				rot.Y += heading * Speed * dt;
			}

			Vector3 camLeft = new Vector3(-1, 0, 0);
			Vector3 camFwdTmp = new Vector3(0, 0, 1);
			camFwdTmp = RotateX(camFwdTmp, -rot.X);
			camLeft = RotateX(camLeft, -rot.X);
			camFwdTmp = RotateY(camFwdTmp, rot.Y);
			camLeft = RotateY(camLeft, rot.Y);
			Rotation = MathHelper.RadiansToDegrees(1f) * rot;

			if (IsActive)
			{
				var camUpTemp = -Vector3.Cross(camFwdTmp, camLeft);
				Position += axisLeft * camLeft * Speed * dt;
				Position += axisUp * camUpTemp * Speed * dt;
				Position += axisFwd * camFwdTmp * Speed * dt;
			}
			camFwd = camFwdTmp;
		}

		public void KeyChange(Keys key, bool pressed)
		{
			var delta = pressed ? 1f : 0f;
			switch (key)
			{
				case Keys.W: axisFwd = delta; Run(pressed); break;
				case Keys.S: axisFwd = -delta; Run(pressed); break;
				case Keys.A: axisLeft = delta; Run(pressed); break;
				case Keys.D: axisLeft = -delta; Run(pressed); break;
				case Keys.Q: axisUp = delta; Run(pressed); break;
				case Keys.E: axisUp = -delta; Run(pressed); break;
				case Keys.H: heading = delta; Run(pressed); break;
				case Keys.F: heading = -delta; Run(pressed); break;
				case Keys.T: tilt = delta; Run(pressed); break;
				case Keys.G: tilt = -delta; Run(pressed); break;
				case Keys.Oemplus:
				case Keys.Add: if(pressed) Speed *= 2.0f; break;
				case Keys.OemMinus:
				case Keys.Subtract: if (pressed) Speed /= 2.0f; break;
			}
		}

		private Vector3 camFwd = new Vector3(0, 0, 1);
		private float heading = 0f;
		private float tilt = 0f;
		private float axisLeft = 0f;
		private float axisFwd = 0f;
		private float axisUp = 0f;
		private float lastMouseX = 0;
		private float lastMouseY = 0;
		private Stopwatch time = new Stopwatch();
		private float lastTime = 0f;

		private void Run(bool running)
		{
			if (running)
			{
				if (!IsActive)
				{
					time.Restart();
					lastTime = 0f;
				}
			}
			else
			{
				bool anyCameraKeyDown = 0f != axisLeft || 0f != axisFwd || 0f != axisUp || 0f != heading || 0f != tilt;
				if (!anyCameraKeyDown)
				{
					time.Reset();
					lastTime = 0f;
				}
			}
		}

		private static Vector3 RotateX(Vector3 vec, float radians)
		{
			var rotateXM = Matrix3.CreateRotationX(MathHelper.RadiansToDegrees(radians));
			return Vector3.Transform(vec, rotateXM);
		}

		private static Vector3 RotateY(Vector3 vec, float radians)
		{
			var rotateZM = Matrix3.CreateRotationY(MathHelper.RadiansToDegrees(radians));
			return Vector3.Transform(vec, rotateZM);
		}
	}
}
