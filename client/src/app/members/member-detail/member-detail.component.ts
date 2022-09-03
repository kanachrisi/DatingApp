import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Member} from 'src/app/models/member';
import {MembersService} from 'src/app/services/members.service';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery';

@Component({
  selector: 'member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit 
{
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private memberService: MembersService, private route: ActivatedRoute) { }

  ngOnInit(): void 
  {
    this.loadMember();
    
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
  
  loadMember()
  {
    let username = this.route.snapshot.paramMap.get('username');
    this.memberService.getMember(username).subscribe(member =>
    {
      this.member = member;
      this.galleryImages = this.getImages();
    })
  }

}
