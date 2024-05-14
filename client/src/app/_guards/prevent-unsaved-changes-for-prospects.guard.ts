import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { ProspectiveListComponent } from '../prospectives/prospective-list/prospective-list.component';


@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesForProspectsGuard implements CanDeactivate<unknown> {
  canDeactivate(component: ProspectiveListComponent): boolean {
    if (component.form!.dirty!) {
      return confirm('This form has unsaved data. Moving away from this form without saving data will result in loss of edited data.  Do you want to continue?');
    }
    return true;
  }
}
