import { Component, OnInit, ViewChild } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Member} from 'src/app/_models/member';
import {MembersService} from 'src/app/services/members.service';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';
import {TabDirective, TabsetComponent} from 'ngx-bootstrap/tabs';
import {Message} from 'src/app/_models/message';
import { MessageService } from './../../services/message.service';

@Component({
  selector: 'member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit 
{
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute, private messageService: MessageService) { }

  ngOnInit(): void 
  {
    this.route.data.subscribe(data =>
    {
      this.member = data.member;
    })
    
    this.route.queryParams.subscribe(params =>
    {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
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
    ]
    
    this.galleryImages = this.getImages();
  }
  
  getImages(): NgxGalleryImage[]
  {
    const imagesUrls = [];
    for (const photo of this.member.photos)
    {
      imagesUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imagesUrls;
  }
  
  /* loadMember()
  {
    let username = this.route.snapshot.paramMap.get('username');
    this.memberService.getMember(username).subscribe(member =>
    {
      this.member = member;
      this.galleryImages = this.getImages();
    })
  } */
  
  loadMessages()
  {
    this.messageService.getMessageThread(this.member.username).subscribe(messages =>
    {
      this.messages = messages;
    })
  }
  
  selectTab(tabId: number)
  {
    this.memberTabs.tabs[tabId].active = true;
  }
  
  onTabActivated(data: TabDirective)
  {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0)
    {
      this.loadMessages();
    }
  }

}
