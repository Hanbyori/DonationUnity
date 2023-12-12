# DonationUnity
서드파티(트윕, 투네이션)의 도네이션 정보를 유니티에서 받을 수 있습니다.
</br>
- Alert Box의 URL을 파싱해서 동작합니다.
- 타 플랫폼하고 연동되는 기능이 추가되면 업데이트 하겠습니다.
</br></br>
## ⚙ Require
1. Unity ```2019.x.x``` 이상
2. ```.Net Framwork 4.x``` 또는 ```.Net Standard 2.x```
</br></br>
## 🛠 Install
1. Unity 플러그인 폴더(```Assets/Plugins```)에 ```websocket-sharp.dll```, ```Newtonsoft.Json.dll``` 추가
2. Unity 프로젝트에 스크립트들(```.cs```) 추가
3. 오브젝트에 ```DonationManager``` 컴포넌트 추가
</br></br>
## 📌 How to use
![image](https://github.com/Hanbyori/DonationUnity/assets/20338405/9ada0c85-7128-4e00-9e32-d04ca64b5a99)
</br>
1. 유니티와 연동할 서드파티를 Use 값에 체크하여 사용하세요.
2. Url 부분에 연동할 서드파티 Alert Box URL 주소를 기입하세요.
3. Token과 Version은 자동으로 기입됩니다.
4. Twip의 도네이션은 ```TwipMessage.cs```에서, Toonation의 도네이션은 ```ToonationMessage.cs```에서 관리됩니다.
