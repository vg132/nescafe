using System;

public class Ppu {
  private byte[] bitmapData;
  public byte[] BitmapData {
    get {
      return bitmapData;
    }
  }

  CpuMemory _cpuMemory;
  PpuMemory _memory;

  // PPUCTRL Register flags
  byte flagBaseNameTableAddr;
  byte flagVRamIncrement;
  byte flagPatternTableAddr;
  byte flagBgPatternTableAddr;
  byte flagSpriteSize;
  byte flagMasterSlaveSelect;
  byte flagVBlankNmi;

  // PPUMASK Register flags
  byte flagGreyscale;
  byte flagShowBackgroundLeft;
  byte flagShowSpritesLeft;
  byte flagShowBackground;
  byte flagShowSprites;
  byte flagEmphasizeRed;
  byte flagEmphasizeGreen;
  byte flagEmphasizeBlue;

  // OAMADDR Register
  byte oamAddr;

  public Ppu(Console console) {
    _cpuMemory = console.cpuMemory;
    _memory = console.ppuMemory;
    bitmapData = new byte[256 * 240]; // 720x486 for a NTSC PPU
  }

  public byte[] getScreen() {
    return bitmapData;
  }

  public byte readFromRegister(ushort address) {
    byte data;
    switch (address) {
      case 0x2002: data = readPpuStatus();
        break;
      case 0x2004: data = readOamData();
        break;
      case 0x2007: data = readPpuData();
        break;
      default:
        throw new Exception("Invalid PPU Register read from register: " + address.ToString("X4"));
    }

    return data;
  }

  public void writeToRegister(ushort address, byte data) {
    switch (address) {
      case 0x2000: writePpuCtrl(data);
        break;
      case 0x2001: writePpuMask(data);
        break;
      case 0x2003: writeOamAddr(data);
        break;
      case 0x2004: writeOamData(data);
        break;
      case 0x2005: writePpuScroll(data);
        break;
      case 0x2006: writePpuData(data);
        break;
      case 0x2007: writePpuData(data);
        break;
      default:
        throw new Exception("Invalid PPU Register write to register: " + address.ToString("X4"));
    }
  }
  
  void writePpuCtrl(byte data) {
    flagBaseNameTableAddr = (byte) (data & 0x3);
    flagVRamIncrement = (byte) ((data >> 2) & 1);
    flagPatternTableAddr = (byte) ((data >> 3) & 1);
    flagBgPatternTableAddr = (byte) ((data >> 4) & 1);
    flagSpriteSize = (byte) ((data >> 5) & 1);
    flagMasterSlaveSelect = (byte) ((data >> 6) & 1);
    flagVBlankNmi = (byte) ((data >> 7) & 1);
  }

  void writePpuMask(byte data) {
    flagGreyscale = (byte) (data & 1);
    flagShowBackgroundLeft = (byte) ((data >> 1) & 1);
    flagShowSpritesLeft = (byte) ((data >> 2) & 1);
    flagShowBackground = (byte) ((data >> 3) & 1);
    flagShowSprites = (byte) ((data >> 4) & 1);
    flagEmphasizeRed = (byte) ((data >> 5) & 1);
    flagEmphasizeGreen = (byte) ((data >> 6) & 1);
    flagEmphasizeBlue = (byte) ((data >> 7) & 1);
  }

  void writeOamAddr(byte data) {
    oamAddr = data;
  }

  void writeOamData(byte data) {
    throw new NotImplementedException();
  }

  void writePpuScroll(byte data) {
    throw new NotImplementedException();
  }

  void writePpuAddr(byte data) {
    throw new NotImplementedException();
  }

  void writePpuData(byte data) {
    throw new NotImplementedException();
  }

  void writeOamDma(byte data) {
    throw new NotImplementedException();
  }

  byte readPpuStatus() {
    throw new NotImplementedException();
  }

  byte readOamData() {
    throw new NotImplementedException();
  }

  byte readPpuData() {
    throw new NotImplementedException();
  }
}