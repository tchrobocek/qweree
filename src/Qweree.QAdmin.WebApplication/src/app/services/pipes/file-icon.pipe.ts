import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fileIcon'
})
export class FileIconPipe implements PipeTransform {

  transform(mediaType: string): string {
    if (mediaType === 'application/octet-stream') {
      return 'insert_drive_file';
    }
    if (mediaType === 'application/json') {
      return 'text_snippet';
    }
    if (mediaType.startsWith('plain/')) {
      return 'text_snippet';
    }
    if (mediaType.startsWith('image/')) {
      return 'photo';
    }
    return 'insert_drive_file';
  }

}
