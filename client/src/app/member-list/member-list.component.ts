import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // members$: Observable<Member[]>;
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [{ value: 'male', display: 'Male' }, { value: 'female', display: 'Female' }];

  constructor(private memberService: MembersService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  ngOnInit(): void {
    // this.loadMembers();
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }

  resetFiletrs() {
    this.userParams = new UserParams(this.user);
    this.loadMembers();
  }

  // loadMembers() {
  //   this.memberService.getMembers().subscribe(members => {
  //     this.members = members;
  //   });
  // }

}
