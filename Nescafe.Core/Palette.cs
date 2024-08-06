using System.Drawing;

namespace Nescafe.Core;

public static class Palette
{
	public static Color GetColor(int index) => _palette.Length < index ? _palette.Last() : _palette[index];
	//public static Color GetColor(int index) => _grayPalette.Length < index ? _grayPalette.Last() : _grayPalette[index];
	//public static Color GetColor(int index) => _grayScalePalette.Length < index ? _grayScalePalette.Last() : _grayScalePalette[index];
	//public static Color GetColor(int index) => _egaPalette.Length < index ? _egaPalette.Last() : _egaPalette[index];
	//public static Color GetColor(int index) => _sepiaPalette.Length < index ? _sepiaPalette.Last() : _sepiaPalette[index];
	//public static Color GetColor(int index) => _alternativePalette.Length < index ? _alternativePalette.Last() : _alternativePalette[index];
	//public static Color GetColor(int index) => _gameBoyPalette.Length < index ? _gameBoyPalette.Last() : _gameBoyPalette[index];

	private static readonly Color[] _palette = new[]
	{
		Color.FromArgb(84, 84, 84),
		Color.FromArgb(0, 30, 116),
		Color.FromArgb(8, 16, 144),
		Color.FromArgb(48, 0, 136),
		Color.FromArgb(68, 0, 100),
		Color.FromArgb(92, 0, 48),
		Color.FromArgb(84, 4, 0),
		Color.FromArgb(60, 24, 0),
		Color.FromArgb(32, 42, 0),
		Color.FromArgb(8, 58, 0),
		Color.FromArgb(0, 64, 0),
		Color.FromArgb(0, 60, 0),
		Color.FromArgb(0, 50, 60),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(152, 150, 152),
		Color.FromArgb(8, 76, 196),
		Color.FromArgb(48, 50, 236),
		Color.FromArgb(92, 30, 228),
		Color.FromArgb(136, 20, 176),
		Color.FromArgb(160, 20, 100),
		Color.FromArgb(152, 34, 32),
		Color.FromArgb(120, 60, 0),
		Color.FromArgb(84, 90, 0),
		Color.FromArgb(40, 114, 0),
		Color.FromArgb(8, 124, 0),
		Color.FromArgb(0, 118, 40),
		Color.FromArgb(0, 102, 120),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(236, 238, 236),
		Color.FromArgb(76, 154, 236),
		Color.FromArgb(120, 124, 236),
		Color.FromArgb(176, 98, 236),
		Color.FromArgb(228, 84, 236),
		Color.FromArgb(236, 88, 180),
		Color.FromArgb(236, 106, 100),
		Color.FromArgb(212, 136, 32),
		Color.FromArgb(160, 170, 0),
		Color.FromArgb(116, 196, 0),
		Color.FromArgb(76, 208, 32),
		Color.FromArgb(56, 204, 108),
		Color.FromArgb(56, 180, 204),
		Color.FromArgb(60, 60, 60),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(236, 238, 236),
		Color.FromArgb(168, 204, 236),
		Color.FromArgb(188, 188, 236),
		Color.FromArgb(212, 178, 236),
		Color.FromArgb(236, 174, 236),
		Color.FromArgb(236, 174, 212),
		Color.FromArgb(236, 180, 176),
		Color.FromArgb(228, 196, 144),
		Color.FromArgb(204, 210, 120),
		Color.FromArgb(180, 222, 120),
		Color.FromArgb(168, 226, 144),
		Color.FromArgb(152, 226, 180),
		Color.FromArgb(160, 214, 228),
		Color.FromArgb(160, 162, 160),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0)
	};

	private static readonly Color[] _alternativePalette = {
		Color.FromArgb(124, 124, 124),
		Color.FromArgb(0, 0, 252),
		Color.FromArgb(0, 0, 188),
		Color.FromArgb(68, 40, 188),
		Color.FromArgb(148, 0, 132),
		Color.FromArgb(168, 0, 32),
		Color.FromArgb(168, 16, 0),
		Color.FromArgb(136, 20, 0),
		Color.FromArgb(80, 48, 0),
		Color.FromArgb(0, 120, 0),
		Color.FromArgb(0, 104, 0),
		Color.FromArgb(0, 88, 0),
		Color.FromArgb(0, 64, 88),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(188, 188, 188),
		Color.FromArgb(0, 120, 248),
		Color.FromArgb(0, 88, 248),
		Color.FromArgb(104, 68, 252),
		Color.FromArgb(216, 0, 204),
		Color.FromArgb(228, 0, 88),
		Color.FromArgb(248, 56, 0),
		Color.FromArgb(228, 92, 16),
		Color.FromArgb(172, 124, 0),
		Color.FromArgb(0, 184, 0),
		Color.FromArgb(0, 168, 0),
		Color.FromArgb(0, 168, 68),
		Color.FromArgb(0, 136, 136),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(248, 248, 248),
		Color.FromArgb(60, 188, 252),
		Color.FromArgb(104, 136, 252),
		Color.FromArgb(152, 120, 248),
		Color.FromArgb(248, 120, 248),
		Color.FromArgb(248, 88, 152),
		Color.FromArgb(248, 120, 88),
		Color.FromArgb(252, 160, 68),
		Color.FromArgb(248, 184, 0),
		Color.FromArgb(184, 248, 24),
		Color.FromArgb(88, 216, 84),
		Color.FromArgb(88, 248, 152),
		Color.FromArgb(0, 232, 216),
		Color.FromArgb(120, 120, 120),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(252, 252, 252),
		Color.FromArgb(164, 228, 252),
		Color.FromArgb(184, 184, 248),
		Color.FromArgb(216, 184, 248),
		Color.FromArgb(248, 184, 248),
		Color.FromArgb(248, 164, 192),
		Color.FromArgb(240, 208, 176),
		Color.FromArgb(252, 224, 168),
		Color.FromArgb(248, 216, 120),
		Color.FromArgb(216, 248, 120),
		Color.FromArgb(184, 248, 184),
		Color.FromArgb(184, 248, 216),
		Color.FromArgb(0, 252, 252),
		Color.FromArgb(248, 216, 248),
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(0, 0, 0)
	};

	private static readonly Color[] _gameBoyPalette = {
		Color.FromArgb(0,0,0),
		Color.FromArgb(1,4,1),
		Color.FromArgb(2,8,2),
		Color.FromArgb(4,12,4),
		Color.FromArgb(5,16,5),
		Color.FromArgb(6,20,6),
		Color.FromArgb(8,24,8),
		Color.FromArgb(9,28,9),
		Color.FromArgb(10,32,10),
		Color.FromArgb(12,36,12),
		Color.FromArgb(13,40,13),
		Color.FromArgb(14,44,14),
		Color.FromArgb(16,48,16),
		Color.FromArgb(17,52,17),
		Color.FromArgb(18,56,18),
		Color.FromArgb(20,60,20),
		Color.FromArgb(21,64,21),
		Color.FromArgb(22,68,22),
		Color.FromArgb(24,72,24),
		Color.FromArgb(25,76,25),
		Color.FromArgb(26,80,26),
		Color.FromArgb(28,84,28),
		Color.FromArgb(29,88,29),
		Color.FromArgb(30,92,30),
		Color.FromArgb(32,96,32),
		Color.FromArgb(33,100,33),
		Color.FromArgb(34,104,34),
		Color.FromArgb(36,108,36),
		Color.FromArgb(37,112,37),
		Color.FromArgb(38,116,38),
		Color.FromArgb(40,120,40),
		Color.FromArgb(41,124,41),
		Color.FromArgb(42,128,42),
		Color.FromArgb(44,132,44),
		Color.FromArgb(45,136,45),
		Color.FromArgb(46,140,46),
		Color.FromArgb(48,144,48),
		Color.FromArgb(49,148,49),
		Color.FromArgb(50,152,50),
		Color.FromArgb(52,156,52),
		Color.FromArgb(53,160,53),
		Color.FromArgb(54,164,54),
		Color.FromArgb(56,168,56),
		Color.FromArgb(57,172,57),
		Color.FromArgb(58,176,58),
		Color.FromArgb(60,180,60),
		Color.FromArgb(61,184,61),
		Color.FromArgb(62,188,62),
		Color.FromArgb(64,192,64),
		Color.FromArgb(65,196,65),
		Color.FromArgb(66,200,66),
		Color.FromArgb(68,204,68),
		Color.FromArgb(69,208,69),
		Color.FromArgb(70,212,70),
		Color.FromArgb(72,216,72),
		Color.FromArgb(73,220,73),
		Color.FromArgb(74,224,74),
		Color.FromArgb(76,228,76),
		Color.FromArgb(77,232,77),
		Color.FromArgb(78,236,78),
		Color.FromArgb(80,240,80),
		Color.FromArgb(81,244,81),
		Color.FromArgb(82,248,82),
		Color.FromArgb(84,252,84)
	};

	private static readonly Color[] _egaPalette = {
		Color.FromArgb(0x00, 0x00, 0x00), // Black
		Color.FromArgb(0x00, 0x00, 0xAA), // Blue
		Color.FromArgb(0x00, 0xAA, 0x00), // Green
		Color.FromArgb(0x00, 0xAA, 0xAA), // Cyan
		Color.FromArgb(0xAA, 0x00, 0x00), // Red
		Color.FromArgb(0xAA, 0x00, 0xAA), // Magenta
		Color.FromArgb(0xAA, 0x55, 0x00), // Brown
		Color.FromArgb(0xAA, 0xAA, 0xAA), // Light Gray
		Color.FromArgb(0x55, 0x55, 0x55), // Dark Gray
		Color.FromArgb(0x55, 0x55, 0xFF), // Light Blue
		Color.FromArgb(0x55, 0xFF, 0x55), // Light Green
		Color.FromArgb(0x55, 0xFF, 0xFF), // Light Cyan
		Color.FromArgb(0xFF, 0x55, 0x55), // Light Red
		Color.FromArgb(0xFF, 0x55, 0xFF), // Light Magenta
		Color.FromArgb(0xFF, 0xFF, 0x55), // Yellow
		Color.FromArgb(0xFF, 0xFF, 0xFF), // White
		Color.FromArgb(0x00, 0x00, 0x55), // Dark Blue
		Color.FromArgb(0x00, 0x55, 0x00), // Dark Green
		Color.FromArgb(0x00, 0x55, 0x55), // Dark Cyan
		Color.FromArgb(0x55, 0x00, 0x00), // Dark Red
		Color.FromArgb(0x55, 0x00, 0x55), // Dark Magenta
		Color.FromArgb(0x55, 0x55, 0x00), // Olive
		Color.FromArgb(0x80, 0x80, 0x80), // Gray
		Color.FromArgb(0x80, 0x80, 0xFF), // Pale Blue
		Color.FromArgb(0x80, 0xFF, 0x80), // Pale Green
		Color.FromArgb(0x80, 0xFF, 0xFF), // Pale Cyan
		Color.FromArgb(0xFF, 0x80, 0x80), // Pale Red
		Color.FromArgb(0xFF, 0x80, 0xFF), // Pale Magenta
		Color.FromArgb(0xFF, 0xFF, 0x80), // Pale Yellow
		Color.FromArgb(0xC0, 0xC0, 0xC0), // Pale Gray
		Color.FromArgb(0xC0, 0xA0, 0x80), // Pale Orange
		Color.FromArgb(0xFF, 0xA0, 0xA0), // Light Orange
		Color.FromArgb(0x80, 0x40, 0x00), // Dark Brown
		Color.FromArgb(0xFF, 0xA5, 0x00), // Orange
		Color.FromArgb(0x80, 0x00, 0x80), // Purple
		Color.FromArgb(0x00, 0x80, 0x80), // Teal
		Color.FromArgb(0x00, 0x80, 0xFF), // Aqua
		Color.FromArgb(0x80, 0xFF, 0x00), // Lime
		Color.FromArgb(0xFF, 0x00, 0x80), // Pink
		Color.FromArgb(0xFF, 0x00, 0xFF), // Fuchsia
		Color.FromArgb(0x40, 0x00, 0x40), // Dark Purple
		Color.FromArgb(0x40, 0x40, 0x00), // Olive Drab
		Color.FromArgb(0x40, 0x40, 0x40), // Dark Grayish
		Color.FromArgb(0x40, 0x00, 0x40), // Dark Magenta
		Color.FromArgb(0x40, 0x00, 0x40), // Deep Purple
		Color.FromArgb(0x00, 0x40, 0x00), // Forest Green
		Color.FromArgb(0x40, 0x40, 0x80), // Slate Blue
		Color.FromArgb(0x00, 0x40, 0x40), // Sea Green
		Color.FromArgb(0x00, 0x40, 0x80), // Steel Blue
		Color.FromArgb(0x00, 0x80, 0x40), // Spring Green
		Color.FromArgb(0x40, 0x80, 0x00), // Chartreuse
		Color.FromArgb(0x80, 0x40, 0x40), // Light Coral
		Color.FromArgb(0x80, 0x40, 0x80), // Violet
		Color.FromArgb(0x80, 0x80, 0x40), // Khaki
		Color.FromArgb(0x80, 0x40, 0xC0), // Lavender
		Color.FromArgb(0x80, 0xC0, 0x40), // Light Goldenrod
		Color.FromArgb(0x40, 0xC0, 0x40), // Medium Sea Green
		Color.FromArgb(0xC0, 0x40, 0x40), // Indian Red
		Color.FromArgb(0xC0, 0xC0, 0x40)  // Goldenrod
	};

	private static readonly Color[] _grayPalette =
	{
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(2, 2, 2),
		Color.FromArgb(4, 4, 4),
		Color.FromArgb(6, 6, 6),
		Color.FromArgb(8, 8, 8),
		Color.FromArgb(10, 10, 10),
		Color.FromArgb(12, 12, 12),
		Color.FromArgb(14, 14, 14),
		Color.FromArgb(16, 16, 16),
		Color.FromArgb(18, 18, 18),
		Color.FromArgb(20, 20, 20),
		Color.FromArgb(22, 22, 22),
		Color.FromArgb(24, 24, 24),
		Color.FromArgb(26, 26, 26),
		Color.FromArgb(28, 28, 28),
		Color.FromArgb(30, 30, 30),
		Color.FromArgb(32, 32, 32),
		Color.FromArgb(34, 34, 34),
		Color.FromArgb(36, 36, 36),
		Color.FromArgb(38, 38, 38),
		Color.FromArgb(40, 40, 40),
		Color.FromArgb(42, 42, 42),
		Color.FromArgb(44, 44, 44),
		Color.FromArgb(46, 46, 46),
		Color.FromArgb(48, 48, 48),
		Color.FromArgb(50, 50, 50),
		Color.FromArgb(52, 52, 52),
		Color.FromArgb(54, 54, 54),
		Color.FromArgb(56, 56, 56),
		Color.FromArgb(58, 58, 58),
		Color.FromArgb(60, 60, 60),
		Color.FromArgb(62, 62, 62),
		Color.FromArgb(64, 64, 64),
		Color.FromArgb(66, 66, 66),
		Color.FromArgb(68, 68, 68),
		Color.FromArgb(70, 70, 70),
		Color.FromArgb(72, 72, 72),
		Color.FromArgb(74, 74, 74),
		Color.FromArgb(76, 76, 76),
		Color.FromArgb(78, 78, 78),
		Color.FromArgb(80, 80, 80),
		Color.FromArgb(82, 82, 82),
		Color.FromArgb(84, 84, 84),
		Color.FromArgb(86, 86, 86),
		Color.FromArgb(88, 88, 88),
		Color.FromArgb(90, 90, 90),
		Color.FromArgb(92, 92, 92),
		Color.FromArgb(94, 94, 94),
		Color.FromArgb(96, 96, 96),
		Color.FromArgb(98, 98, 98),
		Color.FromArgb(100, 100, 100),
		Color.FromArgb(102, 102, 102),
		Color.FromArgb(104, 104, 104),
		Color.FromArgb(106, 106, 106),
		Color.FromArgb(108, 108, 108),
		Color.FromArgb(110, 110, 110),
		Color.FromArgb(112, 112, 112),
		Color.FromArgb(114, 114, 114),
		Color.FromArgb(116, 116, 116),
		Color.FromArgb(118, 118, 118),
		Color.FromArgb(120, 120, 120),
		Color.FromArgb(122, 122, 122),
		Color.FromArgb(124, 124, 124),
		Color.FromArgb(126, 126, 126)
	};

	private static readonly Color[] _sepiaPalette = {
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(5, 4, 3),
		Color.FromArgb(10, 9, 7),
		Color.FromArgb(16, 14, 11),
		Color.FromArgb(21, 19, 14),
		Color.FromArgb(27, 24, 18),
		Color.FromArgb(32, 28, 22),
		Color.FromArgb(37, 33, 26),
		Color.FromArgb(43, 38, 29),
		Color.FromArgb(48, 43, 33),
		Color.FromArgb(54, 48, 37),
		Color.FromArgb(59, 52, 41),
		Color.FromArgb(64, 57, 44),
		Color.FromArgb(70, 62, 48),
		Color.FromArgb(75, 67, 52),
		Color.FromArgb(81, 72, 56),
		Color.FromArgb(86, 76, 59),
		Color.FromArgb(91, 81, 63),
		Color.FromArgb(97, 86, 67),
		Color.FromArgb(102, 91, 71),
		Color.FromArgb(108, 96, 74),
		Color.FromArgb(113, 101, 78),
		Color.FromArgb(118, 105, 82),
		Color.FromArgb(124, 110, 86),
		Color.FromArgb(129, 115, 89),
		Color.FromArgb(135, 120, 93),
		Color.FromArgb(140, 125, 97),
		Color.FromArgb(145, 129, 101),
		Color.FromArgb(151, 134, 104),
		Color.FromArgb(156, 139, 108),
		Color.FromArgb(162, 144, 112),
		Color.FromArgb(167, 149, 116),
		Color.FromArgb(172, 153, 119),
		Color.FromArgb(178, 158, 123),
		Color.FromArgb(183, 163, 127),
		Color.FromArgb(189, 168, 131),
		Color.FromArgb(194, 173, 134),
		Color.FromArgb(199, 178, 138),
		Color.FromArgb(205, 182, 142),
		Color.FromArgb(210, 187, 146),
		Color.FromArgb(216, 192, 149),
		Color.FromArgb(221, 197, 153),
		Color.FromArgb(226, 202, 157),
		Color.FromArgb(232, 206, 161),
		Color.FromArgb(237, 211, 164),
		Color.FromArgb(243, 216, 168),
		Color.FromArgb(248, 221, 172),
		Color.FromArgb(253, 226, 176),
		Color.FromArgb(255, 230, 179),
		Color.FromArgb(255, 235, 183),
		Color.FromArgb(255, 240, 187),
		Color.FromArgb(255, 245, 191),
		Color.FromArgb(255, 250, 194),
		Color.FromArgb(255, 255, 198),
		Color.FromArgb(255, 255, 202),
		Color.FromArgb(255, 255, 206),
		Color.FromArgb(255, 255, 209),
		Color.FromArgb(255, 255, 213),
		Color.FromArgb(255, 255, 217),
		Color.FromArgb(255, 255, 221),
		Color.FromArgb(255, 255, 224),
		Color.FromArgb(255, 255, 228),
		Color.FromArgb(255, 255, 232),
		Color.FromArgb(255, 255, 236),
	};

	private static readonly Color[] _grayScalePalette = {
		Color.FromArgb(0, 0, 0),
		Color.FromArgb(4, 4, 4),
		Color.FromArgb(8, 8, 8),
		Color.FromArgb(12, 12, 12),
		Color.FromArgb(16, 16, 16),
		Color.FromArgb(20, 20, 20),
		Color.FromArgb(24, 24, 24),
		Color.FromArgb(28, 28, 28),
		Color.FromArgb(32, 32, 32),
		Color.FromArgb(36, 36, 36),
		Color.FromArgb(40, 40, 40),
		Color.FromArgb(44, 44, 44),
		Color.FromArgb(48, 48, 48),
		Color.FromArgb(52, 52, 52),
		Color.FromArgb(56, 56, 56),
		Color.FromArgb(60, 60, 60),
		Color.FromArgb(64, 64, 64),
		Color.FromArgb(68, 68, 68),
		Color.FromArgb(72, 72, 72),
		Color.FromArgb(76, 76, 76),
		Color.FromArgb(80, 80, 80),
		Color.FromArgb(84, 84, 84),
		Color.FromArgb(88, 88, 88),
		Color.FromArgb(92, 92, 92),
		Color.FromArgb(96, 96, 96),
		Color.FromArgb(100, 100, 100),
		Color.FromArgb(104, 104, 104),
		Color.FromArgb(108, 108, 108),
		Color.FromArgb(112, 112, 112),
		Color.FromArgb(116, 116, 116),
		Color.FromArgb(120, 120, 120),
		Color.FromArgb(124, 124, 124),
		Color.FromArgb(128, 128, 128),
		Color.FromArgb(132, 132, 132),
		Color.FromArgb(136, 136, 136),
		Color.FromArgb(140, 140, 140),
		Color.FromArgb(144, 144, 144),
		Color.FromArgb(148, 148, 148),
		Color.FromArgb(152, 152, 152),
		Color.FromArgb(156, 156, 156),
		Color.FromArgb(160, 160, 160),
		Color.FromArgb(164, 164, 164),
		Color.FromArgb(168, 168, 168),
		Color.FromArgb(172, 172, 172),
		Color.FromArgb(176, 176, 176),
		Color.FromArgb(180, 180, 180),
		Color.FromArgb(184, 184, 184),
		Color.FromArgb(188, 188, 188),
		Color.FromArgb(192, 192, 192),
		Color.FromArgb(196, 196, 196),
		Color.FromArgb(200, 200, 200),
		Color.FromArgb(204, 204, 204),
		Color.FromArgb(208, 208, 208),
		Color.FromArgb(212, 212, 212),
		Color.FromArgb(216, 216, 216),
		Color.FromArgb(220, 220, 220),
		Color.FromArgb(224, 224, 224),
		Color.FromArgb(228, 228, 228),
		Color.FromArgb(232, 232, 232),
		Color.FromArgb(236, 236, 236),
		Color.FromArgb(240, 240, 240),
		Color.FromArgb(244, 244, 244),
		Color.FromArgb(248, 248, 248),
		Color.FromArgb(252, 252, 252)
	};
}
