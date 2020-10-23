Shaders may be suffixed with _F,_T,_FT, etc.
the suffix is shorthand for describing the shader functionality
_F = FOG
_T = TRANSPARENCY (transparency, if _T is absent, shader will always output alpha 1.0)

The prefix of the name (Lerp, Static, etc) describes their ability to dynamically update the mesh data.
E.G, uniform arrays of model matrices, or in the case of points, two arrays of points for linear interpolation