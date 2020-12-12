import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filename'
})
export class FilenamePipe implements PipeTransform {

  transform(value: string): string {
    const slashIndex = value.lastIndexOf('/');
    return value.substring(slashIndex + 1);
  }

}
