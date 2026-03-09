import { ChangePasswordForm } from './ChangePasswordForm'
import { ProfileForm } from './ProfileForm'

interface ProfilePageProps {
  readonly onBack: () => void
}

export function ProfilePage({ onBack }: ProfilePageProps) {
  return (
    <div className="profile-page">
      <div className="profile-header">
        <button type="button" className="btn btn-secondary" onClick={onBack}>
          Back to Todos
        </button>
        <h2 className="profile-title">Profile Settings</h2>
      </div>

      <ProfileForm />
      <ChangePasswordForm />
    </div>
  )
}
