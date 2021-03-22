import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit, OnDestroy {

  @Input() showSidebar: boolean = false;
  @Output() sidebarStateChanged = new EventEmitter<boolean>();

  currentUser$: Observable<User>;


  constructor(private accountService: AccountService, private router: Router) { }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
  }

  show() {
    this.showSidebar = !this.showSidebar;
    this.sidebarStateChanged.emit(this.showSidebar);
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  ngOnDestroy(): void {
  }

}
