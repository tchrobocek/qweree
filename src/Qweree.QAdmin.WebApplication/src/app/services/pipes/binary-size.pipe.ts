import { Pipe, PipeTransform} from '@angular/core';

export type ByteUnit = 'B' | 'kB' | 'KB' | 'MB' | 'GB' | 'TB';

@Pipe({
  name: 'bytes',
})
export class BinarySizePipe implements PipeTransform {
  static formats: { [key: string]: { max: number; prev?: ByteUnit } } = {
    B: { max: 1024 },
    kB: { max: Math.pow(1024, 2), prev: 'B' },
    KB: { max: Math.pow(1024, 2), prev: 'B' }, // Backward compatible
    MB: { max: Math.pow(1024, 3), prev: 'kB' },
    GB: { max: Math.pow(1024, 4), prev: 'MB' },
    TB: { max: Number.MAX_SAFE_INTEGER, prev: 'GB' },
  };

  static formatResult(result: number, unit: string): string {
    return `${result} ${unit}`;
  }

  static calculateResult(format: { max: number; prev?: ByteUnit }, bytes: number) {
    const prev = format.prev ? BinarySizePipe.formats[format.prev] : undefined;
    return prev ? bytes / prev.max : bytes;
  }

  static toDecimal(value: number, decimal: number): number {
    return Math.round(value * Math.pow(10, decimal)) / Math.pow(10, decimal);
  }


  transform(input: any, decimal: number = 0, from: ByteUnit = 'B', to?: ByteUnit): any {

    let bytes = input;
    let unit = from;
    while (unit !== 'B') {
      bytes *= 1024;
      unit = BinarySizePipe.formats[unit].prev;
    }

    if (to) {
      const format = BinarySizePipe.formats[to];

      const result = BinarySizePipe.toDecimal(BinarySizePipe.calculateResult(format, bytes), decimal);

      return BinarySizePipe.formatResult(result, to);
    }

    for (const key in BinarySizePipe.formats) {
      if (BinarySizePipe.formats.hasOwnProperty(key)) {
        const format = BinarySizePipe.formats[key];
        if (bytes < format.max) {
          const result = BinarySizePipe.toDecimal(BinarySizePipe.calculateResult(format, bytes), decimal);

          return BinarySizePipe.formatResult(result, key);
        }
      }
    }
  }
}
