import { Injectable } from '@angular/core';
import { CanDeactivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

export interface CanComponentDeactivate {
  canDeactivate: () => Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree
}

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  constructor(private confirmService : ConfirmService){}

  canDeactivate(
    component: MemberEditComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (component.editForm.dirty) {
      // return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
      return this.confirmService.confirm('Confirmation','Are you sure you want to exit this page without saving!');
    }
    return true;
  }

}
