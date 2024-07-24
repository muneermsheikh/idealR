import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'depNextSeqName'
})
export class DepNextSeqNamePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
