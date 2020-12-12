import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'mediaType'
})
export class MediaTypePipe implements PipeTransform {

  transform(value: string): string {
    if (value === 'application/octet-stream') {
      return 'binary';
    }
    if (value === 'application/json') {
      return 'json';
    }
    if (value.startsWith('plain/')) {
      return value.substring('plain/'.length);
    }
    if (value.startsWith('image/')) {
      return value.substring('image/'.length);
    }
    return value;
  }

}
