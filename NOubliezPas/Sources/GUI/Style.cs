using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace NOubliezPas
{
    class GuiStyle
    {
        public Texture[] labelTextures;
        public Texture[] scoreTextures;

        public GuiStyle()
        {
            labelTextures = new Texture[9];
            labelTextures[0] = new Texture("Content/Label/bordhautgauche.png");
            labelTextures[1] = new Texture("Content/Label/milieuhaut.png");
            labelTextures[2] = new Texture("Content/Label/bordhautdroit.png");
            labelTextures[3] = new Texture("Content/Label/milieugauche.png");
            labelTextures[4] = new Texture("Content/Label/milieu.png");
            labelTextures[5] = new Texture("Content/Label/milieudroit.png");
            labelTextures[6] = new Texture("Content/Label/bordbasgauche.png");
            labelTextures[7] = new Texture("Content/Label/milieubas.png");
            labelTextures[8] = new Texture("Content/Label/bordbasdroit.png");

            scoreTextures = new Texture[9];
            scoreTextures[0] = new Texture("Content/Score/bordhautgauche.png");
            scoreTextures[1] = new Texture("Content/Score/milieuhaut.png");
            scoreTextures[2] = new Texture("Content/Score/bordhautdroit.png");
            scoreTextures[3] = new Texture("Content/Score/milieugauche.png");
            scoreTextures[4] = new Texture("Content/Score/milieu.png");
            scoreTextures[5] = new Texture("Content/Score/milieudroit.png");
            scoreTextures[6] = new Texture("Content/Score/bordbasgauche.png");
            scoreTextures[7] = new Texture("Content/Score/milieubas.png");
            scoreTextures[8] = new Texture("Content/Score/bordbasdroit.png");
        }
    }
}
