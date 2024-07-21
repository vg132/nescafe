#ifdef GL_ES
  precision mediump float;
#endif

varying vec4 v_color;
varying vec2 v_texCoords;

uniform sampler2D u_sampler2D;
uniform mat4 u_projTrans;

void main(void)
{
  vec2 p = vec2(floor(gl_FragCoord.x), floor(gl_FragCoord.y));
  if (mod(p.y, 5.0)==0.0)
    gl_FragColor = vec4(texture2D(u_sampler2D,v_texCoords).xyz ,1.0);
  else
    gl_FragColor = vec4(0.0,1.0,0.0 ,1.0);
}