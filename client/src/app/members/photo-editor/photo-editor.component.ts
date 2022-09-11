import { Component, Input, OnInit } from '@angular/core';
import {FileUploader} from 'ng2-file-upload';
import {take} from 'rxjs/operators';
import {AccountService} from 'src/app/services/account.service';
import {MembersService} from 'src/app/services/members.service';
import { Member } from 'src/app/_models/member';
import {Photo} from 'src/app/_models/photo';
import {User} from 'src/app/_models/user';
import { environment } from './../../../environments/environment';

@Component({
  selector: 'photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit 
{
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor (private accountService: AccountService, private memberService: MembersService)
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user =>
    {
      this.user = user
    })  
  }

  ngOnInit(): void
  {
    this.initializeUploader();
  }
  
  fileOverBase(event: any)
  {
    this.hasBaseDropzoneOver = event;
  }
  
  setMainPhoto(photo: Photo)
  {
    this.memberService.setMainPhoto(photo.id).subscribe(() =>
    {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p =>
      {
        if (p.isMain)
        {
          p.isMain = false;
        }
        
        if (p.id === photo.id)
        {
          p.isMain = true;
        }
      })
    })
  }
  
  deletePhoto(photoId: number)
  {
    this.memberService.deletePhoto(photoId).subscribe(() =>
    {
      this.member.photos = this.member.photos.filter(p => p.id !== photoId);
    })
  }
  
  initializeUploader()
  {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // 10 MB = 10 * 1024 * 1024
    });
    
    //..this would dertemine if we want to send crenditials 
    //..along side our file and then add some configuration on the
    //..server side. We do not need this since we are sending 
    //..our token in the header
    this.uploader.onAfterAddingFile = (file) =>
    {
      file.withCredentials = false;
    }
    
    this.uploader.onSuccessItem = (item, response, status, headers) =>
    {
      if (response)
      {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

}
