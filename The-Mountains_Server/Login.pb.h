// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Login.proto

#ifndef GOOGLE_PROTOBUF_INCLUDED_Login_2eproto
#define GOOGLE_PROTOBUF_INCLUDED_Login_2eproto

#include <limits>
#include <string>

#include <google/protobuf/port_def.inc>
#if PROTOBUF_VERSION < 3011000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers. Please update
#error your headers.
#endif
#if 3011002 < PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers. Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/port_undef.inc>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_table_driven.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/inlined_string_field.h>
#include <google/protobuf/metadata.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
#define PROTOBUF_INTERNAL_EXPORT_Login_2eproto
PROTOBUF_NAMESPACE_OPEN
namespace internal {
class AnyMetadata;
}  // namespace internal
PROTOBUF_NAMESPACE_CLOSE

// Internal implementation detail -- do not use these members.
struct TableStruct_Login_2eproto {
  static const ::PROTOBUF_NAMESPACE_ID::internal::ParseTableField entries[]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::AuxillaryParseTableField aux[]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::ParseTable schema[2]
    PROTOBUF_SECTION_VARIABLE(protodesc_cold);
  static const ::PROTOBUF_NAMESPACE_ID::internal::FieldMetadata field_metadata[];
  static const ::PROTOBUF_NAMESPACE_ID::internal::SerializationTable serialization_table[];
  static const ::PROTOBUF_NAMESPACE_ID::uint32 offsets[];
};
extern const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_Login_2eproto;
class TouristLogin;
class TouristLoginDefaultTypeInternal;
extern TouristLoginDefaultTypeInternal _TouristLogin_default_instance_;
class UserLogin;
class UserLoginDefaultTypeInternal;
extern UserLoginDefaultTypeInternal _UserLogin_default_instance_;
PROTOBUF_NAMESPACE_OPEN
template<> ::TouristLogin* Arena::CreateMaybeMessage<::TouristLogin>(Arena*);
template<> ::UserLogin* Arena::CreateMaybeMessage<::UserLogin>(Arena*);
PROTOBUF_NAMESPACE_CLOSE

// ===================================================================

class UserLogin :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:UserLogin) */ {
 public:
  UserLogin();
  virtual ~UserLogin();

  UserLogin(const UserLogin& from);
  UserLogin(UserLogin&& from) noexcept
    : UserLogin() {
    *this = ::std::move(from);
  }

  inline UserLogin& operator=(const UserLogin& from) {
    CopyFrom(from);
    return *this;
  }
  inline UserLogin& operator=(UserLogin&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const UserLogin& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const UserLogin* internal_default_instance() {
    return reinterpret_cast<const UserLogin*>(
               &_UserLogin_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    0;

  friend void swap(UserLogin& a, UserLogin& b) {
    a.Swap(&b);
  }
  inline void Swap(UserLogin* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline UserLogin* New() const final {
    return CreateMaybeMessage<UserLogin>(nullptr);
  }

  UserLogin* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<UserLogin>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const UserLogin& from);
  void MergeFrom(const UserLogin& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(UserLogin* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "UserLogin";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_Login_2eproto);
    return ::descriptor_table_Login_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kAccountFieldNumber = 1,
    kPasswordFieldNumber = 2,
  };
  // string account = 1;
  void clear_account();
  const std::string& account() const;
  void set_account(const std::string& value);
  void set_account(std::string&& value);
  void set_account(const char* value);
  void set_account(const char* value, size_t size);
  std::string* mutable_account();
  std::string* release_account();
  void set_allocated_account(std::string* account);
  private:
  const std::string& _internal_account() const;
  void _internal_set_account(const std::string& value);
  std::string* _internal_mutable_account();
  public:

  // string password = 2;
  void clear_password();
  const std::string& password() const;
  void set_password(const std::string& value);
  void set_password(std::string&& value);
  void set_password(const char* value);
  void set_password(const char* value, size_t size);
  std::string* mutable_password();
  std::string* release_password();
  void set_allocated_password(std::string* password);
  private:
  const std::string& _internal_password() const;
  void _internal_set_password(const std::string& value);
  std::string* _internal_mutable_password();
  public:

  // @@protoc_insertion_point(class_scope:UserLogin)
 private:
  class _Internal;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr account_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr password_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  friend struct ::TableStruct_Login_2eproto;
};
// -------------------------------------------------------------------

class TouristLogin :
    public ::PROTOBUF_NAMESPACE_ID::Message /* @@protoc_insertion_point(class_definition:TouristLogin) */ {
 public:
  TouristLogin();
  virtual ~TouristLogin();

  TouristLogin(const TouristLogin& from);
  TouristLogin(TouristLogin&& from) noexcept
    : TouristLogin() {
    *this = ::std::move(from);
  }

  inline TouristLogin& operator=(const TouristLogin& from) {
    CopyFrom(from);
    return *this;
  }
  inline TouristLogin& operator=(TouristLogin&& from) noexcept {
    if (GetArenaNoVirtual() == from.GetArenaNoVirtual()) {
      if (this != &from) InternalSwap(&from);
    } else {
      CopyFrom(from);
    }
    return *this;
  }

  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* descriptor() {
    return GetDescriptor();
  }
  static const ::PROTOBUF_NAMESPACE_ID::Descriptor* GetDescriptor() {
    return GetMetadataStatic().descriptor;
  }
  static const ::PROTOBUF_NAMESPACE_ID::Reflection* GetReflection() {
    return GetMetadataStatic().reflection;
  }
  static const TouristLogin& default_instance();

  static void InitAsDefaultInstance();  // FOR INTERNAL USE ONLY
  static inline const TouristLogin* internal_default_instance() {
    return reinterpret_cast<const TouristLogin*>(
               &_TouristLogin_default_instance_);
  }
  static constexpr int kIndexInFileMessages =
    1;

  friend void swap(TouristLogin& a, TouristLogin& b) {
    a.Swap(&b);
  }
  inline void Swap(TouristLogin* other) {
    if (other == this) return;
    InternalSwap(other);
  }

  // implements Message ----------------------------------------------

  inline TouristLogin* New() const final {
    return CreateMaybeMessage<TouristLogin>(nullptr);
  }

  TouristLogin* New(::PROTOBUF_NAMESPACE_ID::Arena* arena) const final {
    return CreateMaybeMessage<TouristLogin>(arena);
  }
  void CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) final;
  void CopyFrom(const TouristLogin& from);
  void MergeFrom(const TouristLogin& from);
  PROTOBUF_ATTRIBUTE_REINITIALIZES void Clear() final;
  bool IsInitialized() const final;

  size_t ByteSizeLong() const final;
  const char* _InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) final;
  ::PROTOBUF_NAMESPACE_ID::uint8* _InternalSerialize(
      ::PROTOBUF_NAMESPACE_ID::uint8* target, ::PROTOBUF_NAMESPACE_ID::io::EpsCopyOutputStream* stream) const final;
  int GetCachedSize() const final { return _cached_size_.Get(); }

  private:
  inline void SharedCtor();
  inline void SharedDtor();
  void SetCachedSize(int size) const final;
  void InternalSwap(TouristLogin* other);
  friend class ::PROTOBUF_NAMESPACE_ID::internal::AnyMetadata;
  static ::PROTOBUF_NAMESPACE_ID::StringPiece FullMessageName() {
    return "TouristLogin";
  }
  private:
  inline ::PROTOBUF_NAMESPACE_ID::Arena* GetArenaNoVirtual() const {
    return nullptr;
  }
  inline void* MaybeArenaPtr() const {
    return nullptr;
  }
  public:

  ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadata() const final;
  private:
  static ::PROTOBUF_NAMESPACE_ID::Metadata GetMetadataStatic() {
    ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&::descriptor_table_Login_2eproto);
    return ::descriptor_table_Login_2eproto.file_level_metadata[kIndexInFileMessages];
  }

  public:

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  enum : int {
    kAccountFieldNumber = 1,
  };
  // string account = 1;
  void clear_account();
  const std::string& account() const;
  void set_account(const std::string& value);
  void set_account(std::string&& value);
  void set_account(const char* value);
  void set_account(const char* value, size_t size);
  std::string* mutable_account();
  std::string* release_account();
  void set_allocated_account(std::string* account);
  private:
  const std::string& _internal_account() const;
  void _internal_set_account(const std::string& value);
  std::string* _internal_mutable_account();
  public:

  // @@protoc_insertion_point(class_scope:TouristLogin)
 private:
  class _Internal;

  ::PROTOBUF_NAMESPACE_ID::internal::InternalMetadataWithArena _internal_metadata_;
  ::PROTOBUF_NAMESPACE_ID::internal::ArenaStringPtr account_;
  mutable ::PROTOBUF_NAMESPACE_ID::internal::CachedSize _cached_size_;
  friend struct ::TableStruct_Login_2eproto;
};
// ===================================================================


// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
// UserLogin

// string account = 1;
inline void UserLogin::clear_account() {
  account_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline const std::string& UserLogin::account() const {
  // @@protoc_insertion_point(field_get:UserLogin.account)
  return _internal_account();
}
inline void UserLogin::set_account(const std::string& value) {
  _internal_set_account(value);
  // @@protoc_insertion_point(field_set:UserLogin.account)
}
inline std::string* UserLogin::mutable_account() {
  // @@protoc_insertion_point(field_mutable:UserLogin.account)
  return _internal_mutable_account();
}
inline const std::string& UserLogin::_internal_account() const {
  return account_.GetNoArena();
}
inline void UserLogin::_internal_set_account(const std::string& value) {
  
  account_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void UserLogin::set_account(std::string&& value) {
  
  account_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:UserLogin.account)
}
inline void UserLogin::set_account(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  
  account_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:UserLogin.account)
}
inline void UserLogin::set_account(const char* value, size_t size) {
  
  account_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:UserLogin.account)
}
inline std::string* UserLogin::_internal_mutable_account() {
  
  return account_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* UserLogin::release_account() {
  // @@protoc_insertion_point(field_release:UserLogin.account)
  
  return account_.ReleaseNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void UserLogin::set_allocated_account(std::string* account) {
  if (account != nullptr) {
    
  } else {
    
  }
  account_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), account);
  // @@protoc_insertion_point(field_set_allocated:UserLogin.account)
}

// string password = 2;
inline void UserLogin::clear_password() {
  password_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline const std::string& UserLogin::password() const {
  // @@protoc_insertion_point(field_get:UserLogin.password)
  return _internal_password();
}
inline void UserLogin::set_password(const std::string& value) {
  _internal_set_password(value);
  // @@protoc_insertion_point(field_set:UserLogin.password)
}
inline std::string* UserLogin::mutable_password() {
  // @@protoc_insertion_point(field_mutable:UserLogin.password)
  return _internal_mutable_password();
}
inline const std::string& UserLogin::_internal_password() const {
  return password_.GetNoArena();
}
inline void UserLogin::_internal_set_password(const std::string& value) {
  
  password_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void UserLogin::set_password(std::string&& value) {
  
  password_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:UserLogin.password)
}
inline void UserLogin::set_password(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  
  password_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:UserLogin.password)
}
inline void UserLogin::set_password(const char* value, size_t size) {
  
  password_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:UserLogin.password)
}
inline std::string* UserLogin::_internal_mutable_password() {
  
  return password_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* UserLogin::release_password() {
  // @@protoc_insertion_point(field_release:UserLogin.password)
  
  return password_.ReleaseNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void UserLogin::set_allocated_password(std::string* password) {
  if (password != nullptr) {
    
  } else {
    
  }
  password_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), password);
  // @@protoc_insertion_point(field_set_allocated:UserLogin.password)
}

// -------------------------------------------------------------------

// TouristLogin

// string account = 1;
inline void TouristLogin::clear_account() {
  account_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline const std::string& TouristLogin::account() const {
  // @@protoc_insertion_point(field_get:TouristLogin.account)
  return _internal_account();
}
inline void TouristLogin::set_account(const std::string& value) {
  _internal_set_account(value);
  // @@protoc_insertion_point(field_set:TouristLogin.account)
}
inline std::string* TouristLogin::mutable_account() {
  // @@protoc_insertion_point(field_mutable:TouristLogin.account)
  return _internal_mutable_account();
}
inline const std::string& TouristLogin::_internal_account() const {
  return account_.GetNoArena();
}
inline void TouristLogin::_internal_set_account(const std::string& value) {
  
  account_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), value);
}
inline void TouristLogin::set_account(std::string&& value) {
  
  account_.SetNoArena(
    &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:TouristLogin.account)
}
inline void TouristLogin::set_account(const char* value) {
  GOOGLE_DCHECK(value != nullptr);
  
  account_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:TouristLogin.account)
}
inline void TouristLogin::set_account(const char* value, size_t size) {
  
  account_.SetNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:TouristLogin.account)
}
inline std::string* TouristLogin::_internal_mutable_account() {
  
  return account_.MutableNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline std::string* TouristLogin::release_account() {
  // @@protoc_insertion_point(field_release:TouristLogin.account)
  
  return account_.ReleaseNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}
inline void TouristLogin::set_allocated_account(std::string* account) {
  if (account != nullptr) {
    
  } else {
    
  }
  account_.SetAllocatedNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), account);
  // @@protoc_insertion_point(field_set_allocated:TouristLogin.account)
}

#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__
// -------------------------------------------------------------------


// @@protoc_insertion_point(namespace_scope)


// @@protoc_insertion_point(global_scope)

#include <google/protobuf/port_undef.inc>
#endif  // GOOGLE_PROTOBUF_INCLUDED_GOOGLE_PROTOBUF_INCLUDED_Login_2eproto
