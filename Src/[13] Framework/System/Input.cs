using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _13__Framework
{
    public class Input
    {
        Dictionary<Keys, bool> InputKeys = new Dictionary<Keys, bool>();
        private Control Con { get; set; }

        private float dt;

        public void Initialize(Control Form)
        {
            Con = Form;

            foreach (Object key in Enum.GetValues(typeof(Keys)))
            {
                InputKeys[(Keys)key] = false;
            }

            Form.KeyDown += HandleKeyDown;
            Form.KeyUp += HandleKeyUp;
        }

        public bool IsKeyDown(Keys key)
        {
            return InputKeys[key];
        }

        public void KeyDown(Keys key)
        {
            InputKeys[key] = true;
        }

        public void KeyUp(Keys key)
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
