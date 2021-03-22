import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;

  constructor(private mmeberService: MembersService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.mmeberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(resposne => {
      this.members = resposne.result;
      this.pagination = resposne.pagination;
    })
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadLikes();
  }


}
