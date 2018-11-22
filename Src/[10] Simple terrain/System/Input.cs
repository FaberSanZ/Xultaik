using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _10__Simple_terrain
{
    class Input
    {
        Dictionary<Keys, bool> InputKeys = new Dictionary<Keys, bool>();

        internal void Initialize(Control Form)
        {
            foreach (var key in Enum.GetValues(typeof(Keys)))
            {
                InputKeys[(Keys)key] = false;
            }

            Form.KeyDown += HandleKeyDown;
            Form.KeyUp += HandleKeyUp;
        }

        internal bool IsKeyDown(Keys key)
        {
            return InputKeys[key];
        }

        internal void KeyDown(Keys key)
        {
            InputKeys[key] = true;
        }

        internal void KeyUp(Keys key)
        {
            InputKeys[key] = false;
        }


        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            KeyDown(e);
        }

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp(e);
        }

        protected virtual void KeyDown(KeyEventArgs e)
        {
            KeyDown(e.KeyCode);
        }

        protected virtual void KeyUp(KeyEventArgs e)
        {
            KeyUp(e.KeyCode);
        }

    }
}
