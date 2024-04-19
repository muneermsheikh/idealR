import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsavedMemberEditGuard: CanDeactivateFn<MemberEditComponent> = (component) => {

  if(component.editForm?.dirty) {
    return confirm('Thiws form has unsaved changes. Are you sure you want to discard the changes?')
  }
  return true;
};
