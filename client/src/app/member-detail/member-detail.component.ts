import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { NgxGalleryImage } from '@kolkov/ngx-gallery';
import { NgxGalleryAnimation } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Message } from '../_models/message';
import { MessageService } from '../_services/message.service';
import { PresenceService } from '../_services/presence.service';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user: User

  constructor(public presence: PresenceService, private memberService: MembersService, private route: ActivatedRoute, private messageService: MessageService, private accountService: AccountService, private router: Router, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })

    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0)
    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls: NgxGalleryImage[] = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });
    }
    return imageUrls;
  }

  // loadMember() {
  //   this.memberService.getMember(this.route.snapshot.paramMap.get('username'))
  //     .subscribe(member => {
  //       this.member = member;
  //       // this.galleryImages = this.getImages();
  //     })
  // }

  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivate(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      // this.loadMessages();
      this.messageService.createHubConnection(this.user, this.member.username)
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  addLike() {
    this.memberService.addLike(this.member.username).subscribe(() => {
      this.toastr.success('You have liked ' + this.member.knownAs);
    })
  }

}
